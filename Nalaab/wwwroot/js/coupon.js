//var dataTable;

//$(document).ready(function () {
//    loadDataTable();
//});


//function loadDataTable() {
//    dataTable = $('#tblData').DataTable({
//        "ajax": {
//            "url": "/Admin/Coupon/GetAll"
//        },
//        "columns": [
//            { "data": "name", "width": "15%" },
//            { "data": "couponType", "width": "15%" },
//            { "data": "discount", "width": "15%" },
//            { "data": "minimumAmount", "width": "15%" },
//            { "data": "isActive", "width": "15%" },
//            {
//                "data": "id",
//                "render": function (data) {
//                    return `
//                            <div class="text-center">
//                                <a href="/Admin/Coupon/Upsert/${data}" class="btn btn-success text-white" style="cursor:pointer">
//                                    <i class="fas fa-edit"></i> 
//                                </a>
//                                <a onclick=Delete("/Admin/Coupon/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer">
//                                    <i class="fas fa-trash-alt"></i> 
//                                </a>
//                            </div>
//                           `;
//                }, "width": "40%"
//            }
//        ]
//    });
//}

//function Delete(url) {
//    swal({
//        title: "هل أنت متأكد أنك تريد الحذف ؟",
//        text: "لن تستطيع إستعادة ما حذفت",
//        icon: "warning",
//        buttons: true,
//        dangerMode: true
//    }).then((willDelete) => {
//        if (willDelete) {
//            $.ajax({
//                type: "DELETE",
//                url: url,
//                success: function (data) {
//                    if (data.success) {
//                        toastr.success(data.message);
//                        dataTable.ajax.reload();
//                    }
//                    else {
//                        toastr.error(data.message);
//                    }
//                }
//            });
//        }
//    });
//}