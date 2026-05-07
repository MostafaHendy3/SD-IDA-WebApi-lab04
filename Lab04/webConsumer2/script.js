function getAll() {
  var xhr = new XMLHttpRequest();
  xhr.open("GET", "http://localhost:5279/api/Department/getall", true);
  xhr.setRequestHeader("Content-Type", "application/json;");
  xhr.onreadystatechange = function () {
    if (xhr.readyState === 4) {
      if (xhr.status >= 200 && xhr.status < 300) {
        console.log(xhr.responseText);
        var departments = JSON.parse(xhr.responseText);
        displayDepartments(departments);
      }
    }
  };
  xhr.send();
}
function displayDepartments(departments) {
  var tableBody = document.getElementById("departmentsTableBody");
  tableBody.innerHTML = "";
  departments.forEach(function (department) {
    var row = document.createElement("tr");
    var studentNames =
      department.studentsNames && department.studentsNames.length > 0
        ? department.studentsNames.join(", ")
        : "No students";
    row.innerHTML = `
      <td>${department.departmentName}</td>
      <td>${studentNames}</td>
      <td>${department.countStd || 0}</td>
      <td>${department.msg || "N/A"}</td>
    `;
    tableBody.appendChild(row);
  });
}
