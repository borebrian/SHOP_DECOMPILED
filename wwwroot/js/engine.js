$("#search_allbrands").keyup(function () {
	$(this).val($(this).val().toUpperCase());

	$("#already_holder").removeAttr("hidden");

	var input, filter, table, tr, td, i, txtValue;
	input = document.getElementById("search_allbrands");
	//input = id;search_sold_items sold_items
	filter = input.value.toUpperCase();
	table = document.getElementById("allbrands");
	tr = table.getElementsByTagName("tr");
	for (i = 0; i < tr.length; i++) {
		td = tr[i].getElementsByTagName("td")[0];
		if (td) {
			txtValue = td.textContent || td.innerText;
			if (txtValue.toUpperCase().indexOf(filter) > -1) {
				tr[i].style.display = "";

			} else {
				tr[i].style.display = "none";
				//$("#already_holder").hide();
			}
		}
	}
	//var value = this.value.toLowerCase().trim();
	//$("#already_holder").removeAttr("hidden");

	//$("#already_added tr").each(function (index) {
	//    if (!index) return;
	//    $(this).find("td").each(function () {
	//        var id = $(this).text().toLowerCase().trim();
	//        var not_found = (id.indexOf(value) == -1);
	//        $(this).closest('tr').toggle(!not_found);

	//        return not_found;
	//    });
	//alert("hhjghgjh");
	//});
});
$("#search_allbrands").keyup(function () {
	$(this).val($(this).val().toUpperCase());

	$("#already_holder").removeAttr("hidden");

	var input, filter, table, tr, td, i, txtValue;
	input = document.getElementById("search_allbrands");
	//input = id;search_sold_items sold_items
	filter = input.value.toUpperCase();
	table = document.getElementById("allbrands_2");
	tr = table.getElementsByTagName("tr");
	for (i = 0; i < tr.length; i++) {
		td = tr[i].getElementsByTagName("td")[1];
		if (td) {
			txtValue = td.textContent || td.innerText;
			if (txtValue.toUpperCase().indexOf(filter) > -1) {
				tr[i].style.display = "";

			} else {
				tr[i].style.display = "none";
				//$("#already_holder").hide();
			}
		}
	}
	//var value = this.value.toLowerCase().trim();
	//$("#already_holder").removeAttr("hidden");

	//$("#already_added tr").each(function (index) {
	//    if (!index) return;
	//    $(this).find("td").each(function () {
	//        var id = $(this).text().toLowerCase().trim();
	//        var not_found = (id.indexOf(value) == -1);
	//        $(this).closest('tr').toggle(!not_found);

	//        return not_found;
	//    });
	//alert("hhjghgjh");
	//});
});
$("#allbrands > tbody > tr").hide().slice(0, 20).show();
$(".show-all").on("click", function () {
	$("tbody > tr", $(this).prev()).show();
});
function openModal(id,id2) {




}
$("#item_name").keyup(function () {
	$(this).val($(this).val().toUpperCase());

	$("#already_holder").removeAttr("hidden");

	var input, filter, table, tr, td, i, txtValue;
	input = document.getElementById("item_name");
	//input = id;search_sold_items sold_items
	filter = input.value.toUpperCase();
	table = document.getElementById("already_added");
	tr = table.getElementsByTagName("tr");
	for (i = 0; i < tr.length; i++) {
		td = tr[i].getElementsByTagName("td")[0];
		if (td) {
			txtValue = td.textContent || td.innerText;
			if (txtValue.toUpperCase().indexOf(filter) > -1) {
				tr[i].style.display = "";

			} else {
				tr[i].style.display = "none";
				//$("#already_holder").hide();
			}
		}
	}
	//var value = this.value.toLowerCase().trim();
	//$("#already_holder").removeAttr("hidden");

	//$("#already_added tr").each(function (index) {
	//    if (!index) return;
	//    $(this).find("td").each(function () {
	//        var id = $(this).text().toLowerCase().trim();
	//        var not_found = (id.indexOf(value) == -1);
	//        $(this).closest('tr').toggle(!not_found);

	//        return not_found;
	//    });
	//alert("hhjghgjh");
	//});

});
function delete_func(id) {
	$('#delete_id').val(id);
	$('#delete_mod').modal('show');
}
$("#creditors").keyup(function () {
	$(this).val($(this).val().toUpperCase());

	//$("#already_holder").removeAttr("hidden");

	var input, filter, table, tr, td, i, txtValue;
	input = document.getElementById("creditors");
	//input = id;search_sold_items sold_items
	filter = input.value.toUpperCase();
	table = document.getElementById("creditors_table");
	tr = table.getElementsByTagName("tr");
	for (i = 0; i < tr.length; i++) {
		td = tr[i].getElementsByTagName("td")[0];
		if (td) {
			txtValue = td.textContent || td.innerText;
			if (txtValue.toUpperCase().indexOf(filter) > -1) {
				tr[i].style.display = "";

			} else {
				tr[i].style.display = "none";
				//$("#already_holder").hide();
			}
		}
	}
	//var value = this.value.toLowerCase().trim();
	//$("#already_holder").removeAttr("hidden");

	//$("#already_added tr").each(function (index) {
	//    if (!index) return;
	//    $(this).find("td").each(function () {
	//        var id = $(this).text().toLowerCase().trim();
	//        var not_found = (id.indexOf(value) == -1);
	//        $(this).closest('tr').toggle(!not_found);

	//        return not_found;
	//    });
	//alert("hhjghgjh");
	//});
});
$("#search_sold_items").keyup(function () {
	$(this).val($(this).val().toUpperCase());

	//$("#already_holder").removeAttr("hidden");

	var input, filter, table, tr, td, i, txtValue;
	input = document.getElementById("creditors");
	//input = id;search_sold_items sold_items
	filter = input.value.toUpperCase();
	table = document.getElementById("creditors_table");
	tr = table.getElementsByTagName("tr");
	for (i = 0; i < tr.length; i++) {
		td = tr[i].getElementsByTagName("td")[1];
		if (td) {
			txtValue = td.textContent || td.innerText;
			if (txtValue.toUpperCase().indexOf(filter) > -1) {
				tr[i].style.display = "";

			} else {
				tr[i].style.display = "none";
				//$("#already_holder").hide();
			}
		}
	}
	//var value = this.value.toLowerCase().trim();
	//$("#already_holder").removeAttr("hidden");

	//$("#already_added tr").each(function (index) {
	//    if (!index) return;
	//    $(this).find("td").each(function () {
	//        var id = $(this).text().toLowerCase().trim();
	//        var not_found = (id.indexOf(value) == -1);
	//        $(this).closest('tr').toggle(!not_found);

	//        return not_found;
	//    });
	//alert("hhjghgjh");
	//});
});

$("#search_sold_items1").keyup(function () {
	$(this).val($(this).val().toUpperCase());

	//$("#already_holder").removeAttr("hidden");

	var input, filter, table, tr, td, i, txtValue;
	input = document.getElementById("search_sold_items1");
	//input = id;search_sold_items sold_items
	filter = input.value.toUpperCase();
	table = document.getElementById("sold_items_1");
	tr = table.getElementsByTagName("tr");
	for (i = 0; i < tr.length; i++) {
		td = tr[i].getElementsByTagName("td")[1];
		if (td) {
			txtValue = td.textContent || td.innerText;
			if (txtValue.toUpperCase().indexOf(filter) > -1) {
				tr[i].style.display = "";

			} else {
				tr[i].style.display = "none";
				//$("#already_holder").hide();
			}
		}
	}

});
$("#expired_only").click(function () {
	//$("#to_pf_table").tableHTMLExport({
	//    type: 'pdf',
	//    orientation: 'p'
	//});
	var date = $("#search_restock").val();
	$("#sales_made_on").text(date);
	//var img = new Image()
	var doc = new jsPDF();
	//img.src = 'wwwroot/images/logo2.png'
	//doc.addImage(img, 'png', 10, 78, 12, 15)
	var htmlstring = '';
	var tempVarToCheckPageHeight = 0;
	var pageHeight = 0;
	//pageHeight = doc.internal.pageSize.height;
	//specialElementHandlers = {
	//        // element with id of "bypass" - jQuery style selector
	//        '#bypassme': function(element, renderer) {
	//        // true = "handled elsewhere, bypass text extraction"
	//        return true
	//    }
	//};
	//margins = {
	//        top: 10,
	//    bottom: 10,
	//    left: 10,
	//    right: 10,
	//    width: 100
	//};
	var y = 20;
	doc.setLineWidth(2);
	//doc.text(200, y = y + 30, "TOTAL MARKS OF STUDENTS");
	doc.autoTable({
		html: '#expired_itesm_list_table',
	});
	doc.save('ALL_EXPIRED_DETAILS');






});
$("#search_sold_items12").keyup(function () {
	$(this).val($(this).val().toUpperCase());

	//$("#already_holder").removeAttr("hidden");

	var input, filter, table, tr, td, i, txtValue;
	input = document.getElementById("search_sold_items12");
	//input = id;search_sold_items sold_items
	filter = input.value.toUpperCase();
	table = document.getElementById("sold_items_12");
	tr = table.getElementsByTagName("tr");
	for (i = 0; i < tr.length; i++) {
		td = tr[i].getElementsByTagName("td")[0];
		if (td) {
			txtValue = td.textContent || td.innerText;
			if (txtValue.toUpperCase().indexOf(filter) > -1) {
				tr[i].style.display = "";

			} else {
				tr[i].style.display = "none";
				//$("#already_holder").hide();
			}
		}
	}

});





$("#search_expired_table").keyup(function () {
	$(this).val($(this).val().toUpperCase());

	//$("#already_holder").removeAttr("hidden");

	var input, filter, table, tr, td, i, txtValue;
	input = document.getElementById("search_expired_table");
	//input = id;search_sold_items sold_items
	filter = input.value.toUpperCase();
	table = document.getElementById("expired_itesm_list_table");
	tr = table.getElementsByTagName("tr");
	for (i = 0; i < tr.length; i++) {
		td = tr[i].getElementsByTagName("td")[0];
		if (td) {
			txtValue = td.textContent || td.innerText;
			if (txtValue.toUpperCase().indexOf(filter) > -1) {
				tr[i].style.display = "";

			} else {
				tr[i].style.display = "none";
				//$("#already_holder").hide();
			}
		}
	}
	
	$("#exp_search").keyup(function () {
		$(this).val($(this).val().toUpperCase());

		//$("#already_holder").removeAttr("hidden");

		var input, filter, table, tr, td, i, txtValue;
		input = document.getElementById("exp_search");
		//input = id;search_sold_items sold_items
		filter = input.value.toUpperCase();
		table = document.getElementById("expiries");
		tr = table.getElementsByTagName("tr");
		for (i = 0; i < tr.length; i++) {
			td = tr[i].getElementsByTagName("td")[0];
			if (td) {
				txtValue = td.textContent || td.innerText;
				if (txtValue.toUpperCase().indexOf(filter) > -1) {
					tr[i].style.display = "";

				} else {
					tr[i].style.display = "none";
					//$("#already_holder").hide();
				}
			}
		}
		//var value = this.value.toLowerCase().trim();
		//$("#already_holder").removeAttr("hidden");

		//$("#already_added tr").each(function (index) {
		//    if (!index) return;
		//    $(this).find("td").each(function () {
		//        var id = $(this).text().toLowerCase().trim();
		//        var not_found = (id.indexOf(value) == -1);
		//        $(this).closest('tr').toggle(!not_found);

		//        return not_found;
		//    });
		//alert("hhjghgjh");
		//});
	});

});$("#search_restock").keyup(function () {
	$(this).val($(this).val().toUpperCase());

	//$("#already_holder").removeAttr("hidden");

	var input, filter, table, tr, td, i, txtValue;
	input = document.getElementById("search_restock");
	//input = id;search_sold_items sold_items
	filter = input.value.toUpperCase();
	table = document.getElementById("to_restock");
	tr = table.getElementsByTagName("tr");
	for (i = 0; i < tr.length; i++) {
		td = tr[i].getElementsByTagName("td")[1];
		if (td) {
			txtValue = td.textContent || td.innerText;
			if (txtValue.toUpperCase().indexOf(filter) > -1) {
				tr[i].style.display = "";

			} else {
				tr[i].style.display = "none";
				//$("#already_holder").hide();
			}
		}
	}
	//var value = this.value.toLowerCase().trim();
	//$("#already_holder").removeAttr("hidden");

	//$("#already_added tr").each(function (index) {
	//    if (!index) return;
	//    $(this).find("td").each(function () {
	//        var id = $(this).text().toLowerCase().trim();
	//        var not_found = (id.indexOf(value) == -1);
	//        $(this).closest('tr').toggle(!not_found);

	//        return not_found;
	//    });
	//alert("hhjghgjh");
	//});
});
function dispose(id) {
	$("#delete_id").val(id);
	$("#Expired_list").modal('hide');
	$("#delete_mod").modal('show');
	$("#title").text('Are you sure you want to Dispose these items from the system?.');
	$("#body").text(' NB:Make sure you dispose the items from the counter and update the stock count in your system.');


};
function delete_exp(id) {
	$("#delete_id").val(id);
	$("#Expired_list").modal('hide');

	$("#delete_mod").modal('show');
	$("#title").text('Are you sure you want to delete these items from the system?.');
	$("#body").text('');
};

$("#to_restock_pdf").click(function () {
	//$("#to_pf_table").tableHTMLExport({
	//    type: 'pdf',
	//    orientation: 'p'
	//});
	var date = $("#search_restock").val();
	$("#sales_made_on").text(date);
	//var img = new Image()
	var doc = new jsPDF();
	//img.src = 'wwwroot/images/logo2.png'
	//doc.addImage(img, 'png', 10, 78, 12, 15)
	var htmlstring = '';
	var tempVarToCheckPageHeight = 0;
	var pageHeight = 0;
	//pageHeight = doc.internal.pageSize.height;
	//specialElementHandlers = {
	//        // element with id of "bypass" - jQuery style selector
	//        '#bypassme': function(element, renderer) {
	//        // true = "handled elsewhere, bypass text extraction"
	//        return true
	//    }
	//};
	//margins = {
	//        top: 10,
	//    bottom: 10,
	//    left: 10,
	//    right: 10,
	//    width: 100
	//};
	var y = 20;
	doc.setLineWidth(2);
	//doc.text(200, y = y + 30, "TOTAL MARKS OF STUDENTS");
	doc.autoTable({
		html: '#to_restock',
	});
	doc.save('ITEMS_TO_RESTOCK');






});
$("#creditors_pdf").click(function () {
	//$("#to_pf_table").tableHTMLExport({
	//    type: 'pdf',
	//    orientation: 'p'
	//});
	var date = '';
	$("#sales_made_on").text(date);
	//var img = new Image()
	var doc = new jsPDF();
	//img.src = 'wwwroot/images/logo2.png'
	//doc.addImage(img, 'png', 10, 78, 12, 15)
	var htmlstring = '';
	var tempVarToCheckPageHeight = 0;
	var pageHeight = 0;
	//pageHeight = doc.internal.pageSize.height;
	//specialElementHandlers = {
	//        // element with id of "bypass" - jQuery style selector
	//        '#bypassme': function(element, renderer) {
	//        // true = "handled elsewhere, bypass text extraction"
	//        return true
	//    }
	//};
	//margins = {
	//        top: 10,
	//    bottom: 10,
	//    left: 10,
	//    right: 10,
	//    width: 100
	//};
	var y = 20;
	doc.setLineWidth(2);
	//doc.text(200, y = y + 30, "TOTAL MARKS OF STUDENTS");
	doc.autoTable({
		html: '#creditors_table',
	});
	doc.save('ALL_CREDITORS');






});
$("#shop_name").keyup(function () {
	//var value = this.value.toLowerCase().trim();
	this.value = this.value.toUpperCase();
});
$("#name_owner").keyup(function () {
	//var value = this.value.toLowerCase().trim();
	this.value = this.value.toUpperCase();
});

$("#filtered_sold_to_pdf").click(function () {
	//$("#to_pf_table").tableHTMLExport({
	//    type: 'pdf',
	//    orientation: 'p'
	//});
	var date = $("#search_restock").val();
	$("#sales_made_on").text(date);
	//var img = new Image()
	var doc = new jsPDF();
	//img.src = 'wwwroot/images/logo2.png'
	//doc.addImage(img, 'png', 10, 78, 12, 15)
	var htmlstring = '';
	var tempVarToCheckPageHeight = 0;
	var pageHeight = 0;
	//pageHeight = doc.internal.pageSize.height;
	//specialElementHandlers = {
	//        // element with id of "bypass" - jQuery style selector
	//        '#bypassme': function(element, renderer) {
	//        // true = "handled elsewhere, bypass text extraction"
	//        return true
	//    }
	//};
	//margins = {
	//        top: 10,
	//    bottom: 10,
	//    left: 10,
	//    right: 10,
	//    width: 100
	//};
	var y = 20;
	doc.setLineWidth(2);
	//doc.text(200, y = y + 30, "TOTAL MARKS OF STUDENTS");
	doc.autoTable({
		html: '#sold_items_1233',
	});
	doc.save('SALES_FILTERED');






});
//$(".someDiv:visible").each(....);

function calc_total() {
	var sum = 0;
	$(".sub_totals").each(function () {
		sum += parseFloat($(this).text());
	});
	$('#total').text(sum);
	$('#total1').text(sum);
	
}
function calc_total_cost_price() {
	var sum = 0;
	$(".sub_totals_cost_price").each(function () {
		sum += parseFloat($(this).text());
	});

	$('#total_profit').text(sum);
}
	function calc_total_admin() {
	var sum = 0;
		$(".total_per_brand_sold_0").each(function () {
		sum += parseFloat($(this).text());
	});
	
		$('#total_admin_').text(sum);
	
}

function calc_total_cost_price_f() {

var sum = 0;

$(".sub_totals_cost_price_f").each(function () {

		sum += parseFloat($(this).text());
	});
	
	$('#total12_F').text(sum);
	
}


function calc_total_mod2() {
	var sum = 0;
	$(".sub_totals_mod2").each(function () {
		sum += parseFloat($(this).text());
	});
	$('#total123').text(sum);
	
	
}
function calc_total_mod3() {
	var sum = 0;
	$(".sub_totals_mod2").each(function () {
		sum += parseFloat($(this).text());
	});
	$('#total12_').text(sum);
	
	
}