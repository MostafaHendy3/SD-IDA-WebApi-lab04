function addDepartment() {
  var name = document.getElementById("deptName").value;
  var location = document.getElementById("deptLocation").value;
  var phoneNumber = document.getElementById("deptPhone").value;
  var manager = document.getElementById("deptManager").value;
  var msg = document.getElementById("responseMessage");

  var xhr = new XMLHttpRequest();
  xhr.open("POST", "http://localhost:5279/api/Department/add", true);
  xhr.setRequestHeader("Content-Type", "application/json;");
  xhr.onreadystatechange = function () {
    if (xhr.readyState === 4) {
      if (xhr.status >= 200 && xhr.status < 300) {
        msg.textContent = "Department added";
        document.getElementById("deptName").value = "";
        document.getElementById("deptLocation").value = "";
        document.getElementById("deptPhone").value = "";
        document.getElementById("deptManager").value = "";
        
      }
    }
  };
  xhr.send(
    JSON.stringify({
        depName: name,
        depLocation: location,
        depPhoneNumber: phoneNumber,
        depManager: manager,
    })
  );
}
