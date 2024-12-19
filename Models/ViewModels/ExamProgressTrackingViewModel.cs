using System;
using System.Collections.Generic;
using AppDev2Project.Models;

namespace AppDev2Project.Models.ViewModels
{
    public class ExamProgressTrackingViewModel
    {
        public Exam Exam { get; set; }
        public Dictionary<int, StudentProgressInfo> StudentProgress { get; set; }
    }
}
