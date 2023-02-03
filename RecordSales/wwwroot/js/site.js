// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function GotoControllerAsync(url, type, data, includeAntiforgeToken, returnType, successCallback) {
	// type (e.g. post, get etc.)
	// returnType (e.g. json, html etc.)
	// data (json objects)

	const promiseAsync = new Promise((resolve, reject) => {
		resolve('resolve');
	});

	promiseAsync.then((test) => {

		ProcessAsync(url, type, data, includeAntiforgeToken, returnType)
			.then((response) => {

				if (typeof successCallback != 'undefined' && typeof successCallback == 'function') {
					successCallback(response);
				}
			});
	})
}

async function ProcessAsync(url, type, obj, includeAntiforgeToken, returnType) {

	return await $.ajax({
		type: type,
		url: url,
		contentType: includeAntiforgeToken ? 'application/json; charset=utf-8' : 'application/x-www-form-urlencoded; charset=UTF-8',
		data: includeAntiforgeToken ? JSON.stringify(obj) : obj,
		dataType: returnType,
		async: true,
		beforeSend: function (jqXHR, settings) {

			if (includeAntiforgeToken)
				jqXHR.setRequestHeader('RequestVerificationToken', $('.AntiForge' + ' input').val());
		}
	});
}

function validateForm(formId) {
	let isValid = $('#' + formId)[0].checkValidity();
	return isValid;
}

function submitForm(formId, link, objData, callback) {

	var formData = $('#' + formId).serializeArray();
	var datasource = {};
	$(formData).each(function (index, obj) {

		datasource[obj.name] = obj.value;
	});	

	if (objData != null) {
		datasource.Expenses = objData.Expenses;
		datasource.Additionals = objData.Additionals;
		datasource.ExpensesToBeDeleted = expensesToBeDeleted;
		datasource.AdditionalsToBeDeleted = additionalsToBeDeleted;
	}

	console.log(datasource);

	//$.ajax({
	//	type: 'POST',
	//	url: link,
	//	dataType: 'json',
	//	data: datasource,
	//	success: function (response) {

	//		if (callback != null && typeof callback == 'function')
	//			callback(response);
	//	}
	//});

	GotoControllerAsync(link, 'POST', datasource, false, 'json', function (response) {

		if (callback != null && typeof callback == 'function')
			callback(response);
	});
}

function convertToDecimalText(n) {
	let parts = n.toString().split(".");

	const numberPart = parts[0];
	const decimalPart = parts[1];
	const thousands = /\B(?=(\d{3})+(?!\d))/g;

	return numberPart.replace(thousands, ",") + (decimalPart ? "." + decimalPart : "");
}