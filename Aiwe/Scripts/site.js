function submitCommonForm() {
  document.getElementById('commonForm').submit();
}

function isInArray(value, array) {
  return array.indexOf(value) > -1;
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
      && inputs[index].name.indexOf('@ConstantHelper.FilterTimeAppendixFrontName') == -1
    ) {
      msg += inputs[index].name + ': ' + inputs[index].value;
      if (inputs[index].name.indexOf('@ConstantHelper.FilterDateAppendixFrontName') != -1) //If it date type
        if (inputs[index + 1].name.indexOf('@ConstantHelper.FilterTimeAppendixFrontName') != -1) //check the time coutner part (necessarily one index after)
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
    var arr = [];
    var arrOriginals = [];
    var colName = id.substr('live-dd-'.length);
    var selects = document.getElementsByClassName('common-column-dropdown');
    var originals = document.getElementsByClassName('common-column-dropdown-original');
    for (i = 0; i < selects.length; ++i) {
      arr.push(selects[i].value);
      //arr.push(selects[i].options[selects[i].selectedIndex].value);
    }
    for (i = 0; i < originals.length; ++i) {
      arrOriginals.push(originals[i].value);
    }
    var tName = $('#common-table-name').text();

    $.ajax({
      url: '../../../Common/GetLiveDropdownItems/' + tName,
      async: true,
      data: {
        tableName: tName, changedColumnName: colName, originalColumnValues: arrOriginals,
        liveddColumnNames: ldColNames, liveddDataTypes: dataTypes, liveddItems: arr
      },
      traditional: true,
      success: function (data) {
        $.each(data, function (i, obj) {
          $('#live-dd-' + obj.ColumnName).html(obj.HTMLString);
        });
      }
    });

    var inputValue = $('#common-column-dropdown-' + colName).val(); //straightforwards        

    //now, the listColumn parts all the list columns affected by this must be found
    $.ajax({
      url: '../../../Common/GetLiveSubcolumns/' + tName,
      async: true,
      data: {
        tableName: tName, changedColumnName: colName,
        changedColumnValue: inputValue
      },
      traditional: true,
      success: function (data) {
        $.each(data, function (i, obj) {
          if (obj.IsSuccessful) {
            var divId = 'common-subcolumn-div-' + obj.Name;
            var datValId = 'common-subcolumn-datavalue-' + obj.Name;
            $('#' + datValId).html('<input type="hidden" name="' + obj.Name + '" id="common-subcolumn-content-' + obj.Name + '" value="' + obj.DataValue + '"/>');
            $('#' + divId).html(obj.HTMLString);
          }
        });
      }
    });
  });

  $('body').on('click', '.common-subcolumn-button', function (ev) {
    //cancel the default action
    ev.preventDefault();

    //To determine delete or add
    var isAdd = $(this).attr('commonbuttontype') == "add";
    var dNo = isAdd ? 0 : parseInt($(this).attr('commondeleteno'));

    //Get the tableName and the columnName
    var tName = $('#common-table-name').text();
    var cName = $(this).attr('commoncolumnname');

    //get all basic variables needed to load the HTML later
    var lcType = $('#common-subcolumn-span-' + cName).text(); //type of List Column
    var datValId = 'common-subcolumn-datavalue-' + cName;
    var divId = 'common-subcolumn-div-' + cName; //division to replace with the latest HTML
    var content = $('#common-subcolumn-content-' + cName).val(); //content of the column
    var addString = $('#common-subcolumn-addtext-' + cName).val();

    $.ajax({
      url: '../../../Common/GetSubcolumnItems/' + tName,
      async: true,
      data: {
        tableName: tName, columnName: cName, dataValue: content,
        lcType: lcType, deleteNo: dNo,
        addString: addString
      },
      traditional: true,
      success: function (data) {
        if (data.IsSuccessful) {
          $('#' + datValId).html('<input type="hidden" name="' + cName + '" id="common-subcolumn-content-' + cName + '" value="' + data.DataValue + '"/>');
          $('#' + divId).html(data.HTMLString);
        }
      }
    });
  });

  $('body').on('focusout', '.common-subcolumn-input', function () {
    //Get the tableName and the columnName
    var tName = $('#common-table-name').text();
    var cName = $(this).attr('commoncolumnname');

    //Get the input no and type
    var iNo = parseInt($(this).attr('commoninputno'));
    var datValId = 'common-subcolumn-datavalue-' + cName;
    var isText = $(this).attr('commoninputtype') == "text";

    //Get the value of the input
    var inputValue = $(this).val(); //straightforwards

    //get all basic variables needed to load the HTML later
    var lcType = $('#common-subcolumn-span-' + cName).text(); //type of List Column
    var content = $('#common-subcolumn-content-' + cName).val(); //content of the column (before the change)
    var inputPart = $(this).attr('commoninputpart');

    $.ajax({
      url: '../../../Common/UpdateSubcolumnItemsDescription/' + tName,
      async: true,
      data: {
        tableName: tName, columnName: cName, inputPart: inputPart,
        dataValue: content, isText: isText, inputNo: iNo,
        inputValue: inputValue, lcType: lcType
      },
      traditional: true,
      success: function (data) {
        if (data.IsSuccessful) {
          $('#' + datValId).html('<input type="hidden" name="' + cName + '" id="common-subcolumn-content-' + cName + '" value="' + data.DataValue + '"/>');
        }
      }
    });
  });
});
