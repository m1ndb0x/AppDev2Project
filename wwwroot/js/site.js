// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Add functions for exam notifications
function checkNewExams() {
    const exams = document.querySelectorAll('[data-exam-start]');
    exams.forEach(exam => {
        const startTime = new Date(exam.dataset.examStart);
        const now = new Date();
        const timeDiff = Math.abs(now - startTime);
        const minutesDiff = Math.floor(timeDiff / 1000 / 60);

        if (minutesDiff <= 5) {  // Show notification for exams started in last 5 minutes
            showNotification('New exam available: ' + exam.dataset.examTitle, 'info');
        }
    });
}

// Check for new exams every minute
setInterval(checkNewExams, 60000);

// Initial check
document.addEventListener('DOMContentLoaded', checkNewExams);
