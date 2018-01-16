using Aiwe.ActionFilters;
using Aibe.Helpers;
using Aiwe.Models;
using Aiwe.Models.API;
using Aiwe.Models.DB;
using Extension.Database.SqlServer;
using Extension.String;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Aiwe.Controllers {
  //[Authorize]
  [RoutePrefix("api/values")] //cannot be done  
  [ValuesActionFilter]
  public class ValuesController : ApiController {
    CoreDataModel db = new CoreDataModel();

    [HttpPost]
    [Route("getmany")]    
    // GET api/values/getmany
    public HttpResponseMessage GetMany(ClientApiRequest request) {
      return createResponseFor(request, ApiRequestType.SelectMany);
    }

    [HttpPost]
    [Route("get")]
    // GET api/values/get
    public HttpResponseMessage Get(ClientApiRequest request) {
      return createResponseFor(request, ApiRequestType.Select);
    }

    [HttpPost]
    [Route("post")]
    // POST api/values/post
    public HttpResponseMessage Post(ClientApiRequest request) { //In the request for post, there must be all kind of insert into values... which at this moment I will just take everything as they are
      return createResponseFor(request, ApiRequestType.Create);
    }

    [HttpPost]
    [Route("put")]
    // PUT api/values/put
    public HttpResponseMessage Put(ClientApiRequest request) {
      return createResponseFor(request, ApiRequestType.Update);
    }

    [HttpPost]
    [Route("delete")]
    // DELETE api/values/put
    public HttpResponseMessage Delete(ClientApiRequest request) {
      return createResponseFor(request, ApiRequestType.Delete);
    }

    [HttpGet]
    [Route("authenticate")]
    //The only get, which is to authenticate, obtained directly from the link, not from the post
    public HttpResponseMessage Authenticate(string username, string password) {      
      string userNameNormal = Encoding.UTF8.GetString(Convert.FromBase64String(username));
      string passwordNormal = Encoding.UTF8.GetString(Convert.FromBase64String(password));
      bool result = UserHelper.AuthenticateUser(Aiwe.DH.WebApi, userNameNormal, passwordNormal);
      string message = string.Empty;
      bool companySpecificAuthentication = getCompanySpecificMessage(userNameNormal, out message);
      if (!result || !companySpecificAuthentication)
        return createErrorResponse(HttpStatusCode.NotAcceptable, Aibe.LCZ.NFE_UserCannotBeAuthenticated);
      return createAuthenticatedResponse(message);
    }

    #region private methods
    ApplicationDbContext context = new ApplicationDbContext();
    private HttpResponseMessage createAuthenticatedResponse(string message) {
      ApiRequestResult resultApi = new ApiRequestResult();
      resultApi.Message = string.Concat(HttpStatusCode.OK.ToString().ToCamelBrokenString(), "|", message);
      resultApi.Success = true;
      string jsonStr = JsonConvert.SerializeObject(resultApi); //must be assigned AFTER all value assignments are done
      HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
      response.Content = new StringContent(jsonStr, Encoding.UTF8, Aiwe.DH.JsonType);
      return response;
    }

    //Company specific message
    private bool getCompanySpecificMessage(string username, out string message) {
      message = string.Empty;
      ApplicationUser user = context.Users.ToList().FirstOrDefault(x => x.UserName.EqualsIgnoreCaseTrim(username));
      if (user == null)
        return false;
      message = user.Team;
      return true;
    }

    private HttpResponseMessage createResponseFor(ClientApiRequest request, ApiRequestType type) {
      HttpResponseMessage response;
      if (isFiltered(out response))
        return response;
      CheckedClientApiRequest checkedRequest = new CheckedClientApiRequest(request);
      if (!checkedRequest.IsSuccess) {
        StringBuilder errorMsg = new StringBuilder(Aiwe.LCZ.NFE_NotWellFormedRequest);        
        if (!string.IsNullOrWhiteSpace(checkedRequest.ErrorMessage))
          errorMsg.Append(string.Concat("\n", checkedRequest.ErrorMessage));
        return createErrorResponse(HttpStatusCode.BadRequest, errorMsg.ToString());
      }
      string sqlString = string.Empty;
      List<SqlParameter> pars = new List<SqlParameter>();

      switch (type) {
        case ApiRequestType.SelectMany: sqlString = checkedRequest.CreateQueryString();              break;
        case ApiRequestType.Select:     sqlString = checkedRequest.CreateQueryStringSingle();        break;
        case ApiRequestType.Create:     sqlString = checkedRequest.CreateInsertIntoString(out pars); break;
        case ApiRequestType.Update:     sqlString = checkedRequest.CreateUpdateString(out pars);     break;
        case ApiRequestType.Delete:     sqlString = checkedRequest.CreateDeleteString();             break;
        //case ApiRequestType.Insert: //unused
        default:  break;
      }

      if (string.IsNullOrWhiteSpace(sqlString))
        return createErrorResponse(HttpStatusCode.BadRequest, Aiwe.LCZ.NFE_QueryScriptCheckingFails);

      if (pars == null) { //this is by default NOT null, only nullified when parameter creation fails
        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, new Exception(Aiwe.LCZ.NFE_QueryCreationFails));
        return response;
      }

      return type == ApiRequestType.Select || type == ApiRequestType.SelectMany ? 
        createGetTypeResponse(checkedRequest, sqlString) : 
        createPostTypeResponse(checkedRequest, sqlString, pars, checkedRequest.Attachments);
    }

    private Dictionary<ValuesActionFilterResult, HttpStatusCode> resultToCodeDict = new Dictionary<ValuesActionFilterResult, HttpStatusCode>() {
      { ValuesActionFilterResult.ActionNotFound, HttpStatusCode.NotFound },
      { ValuesActionFilterResult.AnonymousUserNotAllowed, HttpStatusCode.Unauthorized },
      { ValuesActionFilterResult.EmptyRequestType, HttpStatusCode.BadRequest },
      { ValuesActionFilterResult.InsufficientAccessRight, HttpStatusCode.Unauthorized },
      { ValuesActionFilterResult.InsufficientAccessRightToAction, HttpStatusCode.Unauthorized },
      { ValuesActionFilterResult.InvalidRequestType, HttpStatusCode.BadRequest },
      { ValuesActionFilterResult.ModelStateInvalid, HttpStatusCode.PreconditionFailed },
      { ValuesActionFilterResult.NullRequest, HttpStatusCode.BadRequest },
      { ValuesActionFilterResult.RequestNotFound, HttpStatusCode.BadRequest },
      { ValuesActionFilterResult.TableDescriptionNotFound, HttpStatusCode.PreconditionFailed },
      { ValuesActionFilterResult.TableNameUnspecified, HttpStatusCode.PreconditionFailed },
      { ValuesActionFilterResult.UnauthorizedRequestType, HttpStatusCode.Unauthorized },
    };

    private bool isFiltered(out HttpResponseMessage message) {
      message = null;
      object errorObject = null;
      if (Request.Properties.TryGetValue(Aiwe.DH.ValuesActionFilterError, out errorObject)) {
        ValuesActionFilterResult error = (ValuesActionFilterResult)errorObject;
        message = createErrorResponse(resultToCodeDict[error], error.ToString().ToCamelBrokenString());
        return true;
      }
      return false;
    }

    private HttpResponseMessage createErrorResponse(HttpStatusCode code, string errorMsg) {
#if DEBUG
      ApiRequestResult result = new ApiRequestResult();
      result.Message = string.Concat(code.ToString().ToCamelBrokenString(), ", ", Aiwe.LCZ.W_AdditionalMessage, ": ", errorMsg);
      string jsonStr = JsonConvert.SerializeObject(result); //must be assigned AFTER all value assignments are done
      HttpResponseMessage errorResponse = this.Request.CreateResponse(HttpStatusCode.OK);
      errorResponse.Content = new StringContent(jsonStr, Encoding.UTF8, Aiwe.DH.JsonType);
#else
      HttpResponseMessage errorResponse = this.Request.CreateErrorResponse(code, errorMsg);
      errorResponse.Content = new StringContent(errorMsg);
#endif
      return errorResponse;
    }

    private HttpResponseMessage createPostTypeResponse(CheckedClientApiRequest checkedRequest, string sqlString, 
      List<SqlParameter> pars = null, ClientApiRequestAttachment[] attachments = null) {
      try {
        ApiRequestResult result = new ApiRequestResult();
        int val = checkedRequest.RequestType == ApiRequestType.Create ?  //if it is create, execute scalar with SELECT SCOPE_IDENTITY() to get the latest cid created
          int.Parse(SQLServerHandler.ExecuteScalar(string.Concat(sqlString, "; SELECT SCOPE_IDENTITY()"), Aibe.DH.DataDBConnectionString, pars).ToString()) :
          SQLServerHandler.ExecuteScript(sqlString, Aibe.DH.DataDBConnectionString, pars);

        result.Success = val > 0;
        if (checkedRequest.RequestType == ApiRequestType.Create)
          result.Message = result.Success ? Aibe.LCZ.NFM_NewItemIsCreated : Aibe.LCZ.NFM_NewItemIsNotCreated;
        else if (checkedRequest.RequestType == ApiRequestType.Update)
          result.Message = result.Success ? String.Format(Aibe.LCZ.M_ItemIsUpdated, checkedRequest.Id) :
            String.Format(Aibe.LCZ.M_ItemIsNotUpdated, checkedRequest.Id);
        else if (checkedRequest.RequestType == ApiRequestType.Delete)
          result.Message = result.Success ? String.Format(Aibe.LCZ.M_ItemIsDeleted, checkedRequest.Id) :
            String.Format(Aibe.LCZ.M_ItemIsNotDeleted, checkedRequest.Id);

        if (!result.Success)
          result.Message = HttpStatusCode.NotModified.ToString().ToCamelBrokenString();
        //Only for create and edit
        else if (attachments != null && attachments.Length > 0 && checkedRequest.RequestType != ApiRequestType.Delete) { //delete cannot have attachment
          //Time to create file based on the attachment
          for (int i = 0; i < attachments.Length; ++i) {
            var attachment = attachments[i];
            if (attachment != null && attachment.IsValid()) {
              if (checkedRequest.Meta != null && !checkedRequest.Meta.IsPictureColumn(attachment.ColumnName))
                continue;
              //if it is NOT a picture link column, then skips this saving process of the attachment
              var fileName = Path.GetFileName(attachment.FileName);
              byte[] fileData = Convert.FromBase64String(attachment.Data);
              var folderPath = System.Web.Hosting.HostingEnvironment.MapPath("~/" + Aibe.DH.DefaultAttachmentFolderName + "/" + checkedRequest.TableName + "/" + 
                (checkedRequest.RequestType == ApiRequestType.Create ? val : checkedRequest.Id)); //creates or edit always table specific and id specific, create uses the newly created Id
              Directory.CreateDirectory(folderPath);
              var path = Path.Combine(folderPath, fileName);
              File.WriteAllBytes(path, fileData);
            }
          }
        }

        string jsonStr = JsonConvert.SerializeObject(result); //must be assigned AFTER all value assignments are done
#if DEBUG
        HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
#else
        HttpResponseMessage response = this.Request.CreateResponse(val > 0 ? HttpStatusCode.OK : HttpStatusCode.NotModified);
#endif
        response.Content = new StringContent(jsonStr, Encoding.UTF8, Aiwe.DH.JsonType);
        return response;

      } catch (Exception ex) {
        string exStr = ex.ToString();
        LogHelper.Error(checkedRequest?.UserName, string.Concat((int)HttpStatusCode.InternalServerError,
          " ", HttpStatusCode.InternalServerError.ToString()), Aiwe.DH.WebApi, Aiwe.DH.WebApiControllerName,
          checkedRequest?.TableName, checkedRequest.RequestType.ToString(), checkedRequest.CreateLogValue(3000),
          exStr);
        return createErrorResponse(HttpStatusCode.InternalServerError, exStr);
      }
    }

    private HttpResponseMessage createGetTypeResponse(CheckedClientApiRequest checkedRequest, string queryString) {
      try {
        DataTable table = SQLServerHandler.GetDataTable(Aibe.DH.DataDBConnectionString, queryString);
        ApiRequestResult result = new ApiRequestResult(checkedRequest, table);

        if (!result.Success)
          return createErrorResponse(HttpStatusCode.BadRequest, Aiwe.LCZ.NFE_BadRequestOnResultMaking);

        //if (result.Rows == null || result.Rows.Count <= 0)
        if ((result.Columns == null || result.Columns.Count <= 0) && (result.Rows == null || result.Rows.Count <= 0))
          return createErrorResponse(HttpStatusCode.NotFound, Aiwe.LCZ.NFE_DataRequestedNotFound);

        string jsonStr = JsonConvert.SerializeObject(result);

        HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
        response.Content = new StringContent(jsonStr, Encoding.UTF8, Aiwe.DH.JsonType);
        return response;
      } catch (Exception ex) {
        string exStr = ex.ToString();
        LogHelper.Error(checkedRequest?.UserName, string.Concat((int)HttpStatusCode.InternalServerError,
          " ", HttpStatusCode.InternalServerError.ToString()), Aiwe.DH.WebApi, Aiwe.DH.WebApiControllerName,
          checkedRequest?.TableName, checkedRequest.RequestType.ToString(), checkedRequest.CreateLogValue(3000),
          exStr);
        return createErrorResponse(HttpStatusCode.BadRequest, exStr);
      }
    }
#endregion
  }
}