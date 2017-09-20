using Aiwe.ActionFilters;
using Aibe.Helpers;
using Aiwe.Models.API;
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
using Aibe;
using Aibe.Models.DB;
using Extension.Database;
using Extension.String;
using Aiwe.Models;

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
      bool result = UserHelper.AuthenticateUser(db, "Web Api", userNameNormal, passwordNormal);
      string message = string.Empty;
      bool companySpecificAuthentication = getFeinmetallMessage(userNameNormal, out message);
      if (!result || !companySpecificAuthentication)
        return createErrorResponse(HttpStatusCode.NotAcceptable, "user cannot be authenticated");
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
      response.Content = new StringContent(jsonStr, Encoding.UTF8, "application/json");
      return response;
    }

    //Company specific message
    private bool getFeinmetallMessage(string username, out string message) {
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
        StringBuilder errorMsg = new StringBuilder("Not-well-formed request");        
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
        return createErrorResponse(HttpStatusCode.BadRequest, "Query script checking fails");

      if (pars == null) { //this is by default NOT null, only nullified when parameter creation fails
        response = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, new Exception("Query creation fails"));
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
      if (Request.Properties.TryGetValue("ValuesActionFilterError", out errorObject)) {
        ValuesActionFilterResult error = (ValuesActionFilterResult)errorObject;
        message = createErrorResponse(resultToCodeDict[error], error.ToString().ToCamelBrokenString());
        return true;
      }
      return false;
    }

    private HttpResponseMessage createErrorResponse(HttpStatusCode code, string errorMsg) {
#if DEBUG
      ApiRequestResult result = new ApiRequestResult();
      result.Message = string.Concat(code.ToString().ToCamelBrokenString(), ", Additional Message: ", errorMsg);
      string jsonStr = JsonConvert.SerializeObject(result); //must be assigned AFTER all value assignments are done
      HttpResponseMessage errorResponse = this.Request.CreateResponse(HttpStatusCode.OK);
      errorResponse.Content = new StringContent(jsonStr, Encoding.UTF8, "application/json");
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
          int.Parse(SQLServerHandler.ExecuteScalar(string.Concat(sqlString, "; SELECT SCOPE_IDENTITY()"), DH.DataDBConnectionString, pars).ToString()) :
          SQLServerHandler.ExecuteScript(sqlString, DH.DataDBConnectionString, pars);

        result.Success = val > 0;
        if (checkedRequest.RequestType == ApiRequestType.Create)
          result.Message = "The new item is " + (val > 0 ? "successfully" : "not") + " created";
        else if (checkedRequest.RequestType == ApiRequestType.Update)
          result.Message = "The item with [Id = " + checkedRequest.Id + "] is " + (val > 0 ? "successfully" : "not") + " updated";
        else if(checkedRequest.RequestType == ApiRequestType.Delete)
          result.Message = "The item with [Id = " + checkedRequest.Id + "] is " + (val > 0 ? "successfully" : "not") + " deleted";

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
              var folderPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Images/" + checkedRequest.TableName + "/" + 
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
        response.Content = new StringContent(jsonStr, Encoding.UTF8, "application/json");
        return response;

      } catch (Exception ex) {
        string exStr = ex.ToString();
        LogHelper.Error(checkedRequest?.UserName, string.Concat((int)HttpStatusCode.InternalServerError,
          " ", HttpStatusCode.InternalServerError.ToString()), "Web Api", "Values",
          checkedRequest?.TableName, checkedRequest.RequestType.ToString(), checkedRequest.CreateLogValue(3000),
          exStr);
        return createErrorResponse(HttpStatusCode.InternalServerError, exStr);
      }
    }

    private HttpResponseMessage createGetTypeResponse(CheckedClientApiRequest checkedRequest, string queryString) {
      try {
        DataTable table = SQLServerHandler.GetDataTable(DH.DataDBConnectionString, queryString);
        ApiRequestResult result = new ApiRequestResult(checkedRequest, table);

        if (!result.Success)
          return createErrorResponse(HttpStatusCode.BadRequest, "Bad request on result making");

        //if (result.Rows == null || result.Rows.Count <= 0)
        if ((result.Columns == null || result.Columns.Count <= 0) && (result.Rows == null || result.Rows.Count <= 0))
          return createErrorResponse(HttpStatusCode.NotFound, "Data requested not found");

        string jsonStr = JsonConvert.SerializeObject(result);

        HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
        response.Content = new StringContent(jsonStr, Encoding.UTF8, "application/json");
        return response;
      } catch (Exception ex) {
        string exStr = ex.ToString();
        LogHelper.Error(checkedRequest?.UserName, string.Concat((int)HttpStatusCode.InternalServerError,
          " ", HttpStatusCode.InternalServerError.ToString()), "Web Api", "Values",
          checkedRequest?.TableName, checkedRequest.RequestType.ToString(), checkedRequest.CreateLogValue(3000),
          exStr);
        return createErrorResponse(HttpStatusCode.BadRequest, exStr);
      }
    }
#endregion
  }
}

//extract JSON items here...
//strings.Add(value);
//return "New Value added: " + request.Value;

// GET api/values

//The request should consists of 4 items: Request Identity, Table Name, table column, table filter, these are all which we need
//Request Identity: must have matching user Id and password
//Must have the table name wanted to be obtained
//Must have table columns, if null, means all
//Must have some sort of filters for the obtained values...

//public IEnumerable<string> GetMany(ClientApiRequest request)
//public string GetMany(ClientApiRequest request) {      
//public IEnumerable<string> Get() {
//  return strings;
//}

//One thing for sure, this type can be any type, doesn't matter!
//string str = JsonConvert.SerializeObject(result, result.GetType(), new JsonSerializerSettings());
//return str;

//// convert string to stream
//byte[] byteArray = Encoding.UTF8.GetBytes(str);
//return byteArray;
//byte[] byteArray = Encoding.ASCII.GetBytes(contents);
//MemoryStream stream = new MemoryStream(byteArray);
//return stream;
////return strings;

//string oldValue = strings[request.Id.Value];
//strings[request.Id.Value] = request.Value;
//return "Old value = " + oldValue + " on Id = " + request.Id.Value + " is updated to New Value = " + request.Value;

//string oldValue = strings[request.Id.Value];
//strings.RemoveAt(request.Id.Value);
//return "Successfully remove Value = " + oldValue + " at Id = " + request.Id.Value;
