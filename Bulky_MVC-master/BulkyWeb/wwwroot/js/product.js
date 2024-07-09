var dataTable;                      // Declare a global dataTable so we can reload and do other stuffs from different funcs 

$(document).ready(function () {     // Gotta mention loadDataTable() in (document).ready() func. WHY?? To  access the classnames after they are loaded
    loadDataTable();                // .EX.#tblData.Also to make sure HTML is fully parsed first.Otherwise, the func can manipulate before loading
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({               // Columns here cannot be more than columns declared in cshtml data-table #tblData header
        "ajax": { url:'/admin/product/getall'},         // This is the URL
        "columns": [
            { data: 'title', "width": "25%" },
            { data: 'isbn', "width": "15%" },
            { data: 'listPrice', "width": "10%" },
            { data: 'author', "width": "15%" },
            { data: 'category.name', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {     // Render two buttons. For edit, /admin/product/upsert?id=${data} to make sure when we press edit, it goes to upsert. The parameter data is the ID
                    return `<div class="w-75 btn-group" role="group">
                     <a href="/admin/product/upsert?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit</a>                   
                     <a onClick=Delete('/admin/product/delete/${data}') class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i> Delete</a>
                    </div>`
                },                              // <i class="bi bi-pencil-square"> pencil icon
                "width": "25%"
            }
        ]
    });
}

function Delete(url) {              // It is called from render onClick=Delete('/admin/product/delete/${data}')
    Swal.fire({                     // Sweet alert
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {       // Get the result
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',     // Send the delete type to Controller
                success: function (data) {
                    dataTable.ajax.reload();        // Refreshing the dataTable
                    toastr.success(data.message);       // Toast to show the result
                }
            })
        }
    })
}