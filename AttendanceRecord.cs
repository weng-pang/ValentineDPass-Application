using System;
using System.Collections.Generic;
using System.Text;

namespace AttLogs
{
    class AttendanceRecord
    {
        private string sdwEnrollNumber = "";

        public string SdwEnrollNumber
        {
            get { return sdwEnrollNumber; }
            set { sdwEnrollNumber = value; }
        }
        private int idwVerifyMode = 0;

        public int IdwVerifyMode
        {
            get { return idwVerifyMode; }
            set { idwVerifyMode = value; }
        }
        private int idwInOutMode = 0;

        public int IdwInOutMode
        {
            get { return idwInOutMode; }
            set { idwInOutMode = value; }
        }
       private int idwYear = 0;

        public int IdwYear
        {
            get { return idwYear; }
            set { idwYear = value; }
        }
        private int idwMonth = 0;

        public int IdwMonth
        {
            get { return idwMonth; }
            set { idwMonth = value; }
        }
        private int idwDay = 0;

        public int IdwDay
        {
            get { return idwDay; }
            set { idwDay = value; }
        }
        private int idwHour = 0;

        public int IdwHour
        {
            get { return idwHour; }
            set { idwHour = value; }
        }
        private int idwMinute = 0;

        public int IdwMinute
        {
            get { return idwMinute; }
            set { idwMinute = value; }
        }
        private int idwSecond = 0;

        public int IdwSecond
        {
            get { return idwSecond; }
            set { idwSecond = value; }
        }
        private int idwWorkcode = 0;

        public int IdwWorkcode
        {
            get { return idwWorkcode; }
            set { idwWorkcode = value; }
        }

    }
}
