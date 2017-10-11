function submitCommonForm() {
  document.getElementById('commonForm').submit();
}

function isInArray(value, array) {
  return array.indexOf(value) > -1;
}

function processLiveDd(element, ldColNames, dataTypes, applyListColumnLoad) {
  var id = element.id;
  var arr = [];
  var arrOriginals = [];
  var colName = id.substr('live-dd-'.length);
  var selects = document.getElementsByClassName('common-column-dropdown');
  var originals = document.getElementsByClassName('common-column-dropdown-original');
  for (i = 0; i < selects.length; ++i) {
    arr.push(selects[i].value);
  }
  for (k = 0; k < originals.length; ++k) {
    arrOriginals.push(originals[k].value);
  }
  var tableName = $('#common-table-name').text();

  $.ajax({
    url: '../../../Common/GetLiveDropDownItems/' + tableName,
    async: true,
    data: {
      tableName: tableName, changedColumnName: colName, originalColumnValues: arrOriginals,
      liveddColumnNames: ldColNames, liveddDataTypes: dataTypes, liveddItems: arr
    },
    traditional: true,
    success: function (data) {
      $.each(data, function (v, obj) {
        $('#live-dd-' + obj.ColumnName).html(obj.ViewString);
      });
    }
  });

  if (applyListColumnLoad) {
    var inputValue = $('#common-column-dropdown-' + colName).val(); //straightforwards        

    //now, the listColumn parts all the list columns affected by this must be found
    $.ajax({
      url: '../../../Common/GetLiveSubcolumns/' + tableName,
      async: true,
      data: {
        tableName: tableName, changedColumnName: colName,
        changedColumnValue: inputValue
      },
      traditional: true,
      success: function (data) {
        $.each(data, function (v, obj) {
          if (obj.IsSuccessful) {
            var datValId = 'common-subcolumn-datavalue-' + obj.Name;
            var divId = 'common-subcolumn-div-' + obj.Name;
            $('#' + datValId).html('<input type="hidden" name="' + obj.Name + '" id="common-subcolumn-content-' + obj.Name + '" value="' + obj.DataValue + '"/>');
            $('#' + divId).html(obj.ViewString);
          }
        });
      }
    });
  }
}

function submitFilterModalForm(submitter, ev, filteredType) {
  inputs = document.getElementsByTagName('input');
  var count = 0;
  var msg = '';
  var debugMsg = '';
  for (index = 0; index < inputs.length; ++index) {
    if (!inputs[index].name.startsWith("__RequestVerificationToken")
      && !inputs[index].classList.contains("uncounted-filter-item")
      && inputs[index].value != ''
      && inputs[index].name.indexOf('@Aibe.DH.FilterTimeAppendixFrontName') == -1
    ) {
      msg += inputs[index].name + ': ' + inputs[index].value;
      if (inputs[index].name.indexOf('@Aibe.DH.FilterDateAppendixFrontName') != -1) //If it date type
        if (inputs[index + 1].name.indexOf('@Aibe.DH.FilterTimeAppendixFrontName') != -1) //check the time coutner part (necessarily one index after)
          msg += ' ' + inputs[index + 1].value; //add the time counterpart
      msg += '\n';
      count++;
    }
  }

  options = document.getElementsByTagName('option');
  for (index = 0; index < options.length; ++index) {
    if (options[index].selected && options[index].value != ''
      && !options[index].classList.contains("uncounted-filter-item")) {
      var pe = options[index].parentElement;
      var peName = pe.attributes.getNamedItem("name");
      var valSelected = options[index].value;
      msg += peName.value + ': ' + valSelected + '\n';
      count++;
    }
  }

  var spanEl = $('#span-for-' + filteredType + '-filter');
  $('#' + filteredType + '-filter-no').val(count);
  $('#' + filteredType + '-filter-msg').val(msg);
  if (count > 0) {
    spanEl.html('<span class="glyphicon glyphicon-ok-circle text-success" title="' + msg + '"></span> <span title="Filter elements">' + count + '</span>');
  } else {
    spanEl.html('');
  }
  if (submitter.id == filteredType + '-filter-use-button')
    document.getElementById('filterForm').submit();
}

function submitFilterForm(pageNo, filteredType) {
  var oldPage = $('#' + filteredType + '-page').val();
  $('#' + filteredType + '-page').val(pageNo);
  document.getElementById('filterForm').submit();
}

function readURL(columnName, input) {
  if (input.files && input.files[0]) {
    var reader = new FileReader();

    reader.onload = function (e) {
      $('#picturedisplay-' + columnName).attr('src', e.target.result);
    }

    reader.readAsDataURL(input.files[0]);
  }
}

//http://stackoverflow.com/questions/857618/javascript-how-to-extract-filename-from-a-file-input-control
$(document).ready(function () {
  $('.common-input-picture').change(function () {
    var val = $(this).val();
    var filename = val.split(/(\\|\/)/g).pop();
    var id = $(this).attr('id');
    var strId = id.substr('picture-'.length);
    var prevFileName = $('#common-string-picture-' + strId).val();
    var fromPrevious = false;

    if (!filename && prevFileName) { //the filename is empty but there is something else before, just assign that something else
      $('#pictureinfotext-' + strId).show();
      filename = prevFileName;
      fromPrevious = true;
    }

    $('#common-string-picture-' + strId).val(filename);
    if (filename) {
      $('#picturedisplay-' + strId).show();
      $('#pictureremove-' + strId).show();
      if(!fromPrevious)
        $('#pictureinfotext-' + strId).hide();
    } else {
      $('#picturedisplay-' + strId).hide();
      $('#pictureremove-' + strId).hide();
    }
    readURL(strId, this);
  })

  $('.common-remove-picture').click(function () {
    var id = $(this).attr('id');
    var strId = id.substr('pictureremove-'.length);
    $('#picturedisplay-' + strId).attr('src', ''); //picture should not be shown, src removed
    $('#picture-' + strId).val('');
    $('#common-string-picture-' + strId).val(''); //the filename emptied
    $('#pictureinfotext-' + strId).hide();
    $('#picturedisplay-' + strId).hide();
    $('#pictureremove-' + strId).hide(); //the remove button must be hidden too
  });


  //Catch all events related to changes http://stackoverflow.com/questions/21215049/disable-text-entry-in-input-type-number
  $('.number-input').on('change keyup', function () {
    var sanitized = $(this).val().replace(/[^0-9]/g, ''); // Remove invalid characters
    $(this).val(sanitized); // Update value
  });

  $('.module-button').click(function () {
    var id = $(this).attr('id');
    var strId = id.substr('module-button-'.length);
    if ($(this).hasClass('glyphicon-chevron-down')) {
      $(this).removeClass('glyphicon-chevron-down');
      $(this).addClass('glyphicon-chevron-up');
      $('#module-' + strId).hide();
    } else {
      $(this).removeClass('glyphicon-chevron-up');
      $(this).addClass('glyphicon-chevron-down');
      $('#module-' + strId).show();
    }
  });

  $('body').on('change keyup', '.live-dd', function () {
    var id = $(this).attr('id');
    var elements = document.getElementsByClassName('live-dd');
    var ldColNames = [];
    var dataTypes = [];
    for (i = 0; i < elements.length; ++i) {
      ldColNames.push(elements[i].id.substr('live-dd-'.length));
      dataTypes.push(elements[i].attributes.getNamedItem('commondatatype').value);
    }
    processLiveDd(this, ldColNames, dataTypes, true);
  });

  $('body').on('click', '.common-subcolumn-button', function (ev) {
    //cancel the default action
    ev.preventDefault();

    //Get the tableName and the columnName
    var tableName = $('#common-table-name').text();

    //Take all info from the ListColumnExtension
    var columnName = $(this).attr('commoncolumnname');
    var isAdd = $(this).attr('commonbuttontype') == "add"; //to determine if it is add or delete
    var deleteNo = isAdd ? 0 : parseInt($(this).attr('commondeleteno'));

    //get all basic variables needed to load the HTML later
    var lcType = $('#common-subcolumn-span-' + columnName).text(); //type of List Column
    var dataValue = $('#common-subcolumn-content-' + columnName).val(); //content of the column

    //get all adds:
    var allAdds = document.getElementsByClassName('common-subcolumn-input-add-' + columnName);
    var addString = '';
    for (i = 0; i < allAdds.length; ++i) {
      if (i > 0)
        addString += '|';
      var subItemType = allAdds[i].attributes.getNamedItem('commonsubitemtype').value; //there are four possible subItemType here
      var subItemVal = allAdds[i].value;
      switch (subItemType) {
        case 'L':
        case 'V': addString += subItemVal; break;
        case 'O':
          if (subItemVal.toString().indexOf('|') !== -1) {
            addString += subItemVal;
          } else {
            addString += subItemVal + '|' + subItemVal;
          }
          break;
        case 'C': addString += subItemVal + '|Yes,No'; break;
        default:
      }
    }

    //AJAX to update dynamically
    $.ajax({
      url: '../../../Common/GetSubcolumnItems/' + tableName,
      async: true,
      data: {
        tableName: tableName, columnName: columnName,
        dataValue: dataValue, lcType: lcType,
        deleteNo: deleteNo, addString: addString
      },
      traditional: true,
      success: function (data) {
        if (data.IsSuccessful) {
          var datValId = 'common-subcolumn-datavalue-' + columnName; //this is where the hidden input value is placed
          var divId = 'common-subcolumn-div-' + columnName; //division to replace with the latest HTML
          $('#' + datValId).html('<input type="hidden" name="' + columnName + '" id="common-subcolumn-content-' + columnName + '" value="' + data.DataValue + '"/>');
          $('#' + divId).html(data.ViewString);
        }
      }
    });
  });

  $('body').on('focusout', '.common-subcolumn-input', function () {
    //Get tableName and the input value obtained for this change
    var tableName = $('#common-table-name').text();
    var inputValue = $(this).val(); //straightforwards

    //All these are obtained from the ListColumnInfoExtension
    var columnName = $(this).attr('commoncolumnname'); //this is the columnName
    var rowNo = parseInt($(this).attr('commonrowno')); //this is the rowNo
    var columnNo = parseInt($(this).attr('commoncolumnno')); //this is the columnNo

    //get all other basic variables needed to load the HTML later
    var lcType = $('#common-subcolumn-span-' + columnName).text(); //type of List Column
    var dataValue = $('#common-subcolumn-content-' + columnName).val(); //content of the column (before the change)

    $.ajax({
      url: '../../../Common/UpdateSubcolumnItemsDescription/' + tableName,
      async: true,
      data: {
        tableName: tableName, columnName: columnName,
        rowNo: rowNo, columnNo: columnNo,
        dataValue: dataValue, inputValue: inputValue,
        lcType: lcType
      },
      traditional: true,
      success: function (data) {
        if (data.IsSuccessful) { //when this is successful, change the hidden input of the ListColumn item here to the new value
          var datValId = 'common-subcolumn-datavalue-' + columnName; //this is a div, inside this contains the hidden input for the listColumn
          $('#' + datValId).html('<input type="hidden" name="' + columnName + '" id="common-subcolumn-content-' + columnName + '" value="' + data.DataValue + '"/>');
        }
      }
    });
  });

  $(window).load(function () {
    // run code
    var elements = document.getElementsByClassName('live-dd');
    var ldColNames = [];
    var dataTypes = [];
    for (i = 0; i < elements.length; ++i) {
      ldColNames.push(elements[i].id.substr('live-dd-'.length));
      dataTypes.push(elements[i].attributes.getNamedItem('commondatatype').value);
    }
    for (j = 0; j < elements.length; ++j) {
      processLiveDd(elements[j], ldColNames, dataTypes, false); //the first time does not apply listColumn loading
    }
  });
});
