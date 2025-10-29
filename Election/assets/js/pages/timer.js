

var time = $("#examtime").val()
$(document).ready(function () {
    if (confirm("هل انت جاهز لبدأ الامتحان؟؟") == true) {

        $('.clock').text(`${time}:00`);
        countdown();
    }
    else {
        var courseid = document.getElementById("courseid").value;
        window.location.href = "/Student/MyCourses";
    }
  });
  var interval;
  
function countdown() {
  

    clearInterval(interval);
    interval = setInterval( function() {
        var timer = $('.clock').html();
        timer = timer.split(':');
        var minutes = timer[0];
        var seconds = timer[1];
        seconds -= 1;
        if (minutes < 0) return;
        else if (seconds < 0 && minutes != 0) {
            minutes -= 1;
            seconds = 59;
        }
        else if (seconds < 10 && length.seconds != 2) seconds = '0' + seconds;
  
        $('.clock').html(minutes + ':' + seconds);
        $('#ttt').val(minutes);

  
        if (minutes == 0 && seconds == 0) {
            clearInterval(interval);
         
            var button = document.getElementById('clickButton');
            button.form.submit();
        }
       
    }, 1000);
  };