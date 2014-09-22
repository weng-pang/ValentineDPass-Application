/**********************************************************
 * Demo for Standalone SDK.Created by Darcy on Oct.15 2009*
***********************************************************/
/**
 * Weng Long Pang 
 * KATS 2014
 * 
 * Valentine D-PASS Project
 * 
 * This application provides access to attendance system introduced since late 2013.
 * 
 * Attendance raw data and processed data will be obtained from this application.
 * 
 * 
 * 
 **/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Text.RegularExpressions;


namespace AttLogs {
  
    public partial class AttLogsMain : Form {
        //Create Standalone SDK class dynamically.
        private Connector attendanceConnection = new Connector();
        List<AttendanceRecord> attendanceRecordList;

        public AttLogsMain() {
            InitializeComponent();
        }

        /*************************************************************************************************
        * Before you refer to this demo,we strongly suggest you read the development manual deeply first.*
        * This part is for demonstrating the communication with your device.                             *
        * ************************************************************************************************/
        #region Communication
        //private int iMachineNumber = 1;//the serial number of the device.After connecting the device ,this value will be changed.

        //If your device supports the TCP/IP communications, you can refer to this.
        //when you are using the tcp/ip communication,you can distinguish different devices by their IP address.
        private void btnConnect_Click(object sender, EventArgs e) 
        {
            if (!Validator.checkIpAddress(txtIP.Text) || !Validator.checkPort(txtPort.Text)){
                MessageBox.Show("IP and Port cannot be null or incorrect", "Error");
                return;
            }

            Cursor = Cursors.WaitCursor;
            if (attendanceConnection.isConnected())
            {
                // Disconnection Procedures
                attendanceConnection.disconnect();
                btnConnect.Text = "Connect";
                lblState.Text = "Current State:DisConnected";
                Cursor = Cursors.Default;
                enableButtons(false);
                return; // Leave the application immediately
            }
            // Connection procedure
            backgroundWorker1.RunWorkerAsync();
            
        }
        #endregion

        /*************************************************************************************************
        * Before you refer to this demo,we strongly suggest you read the development manual deeply first.*
        * This part is for demonstrating operations with(read/get/clear) the attendance records.         *
        * ************************************************************************************************/
        #region AttLogs

        //Download the attendance records from the device(For both Black&White and TFT screen devices).
        private void btnGetGeneralLogData_Click(object sender, EventArgs e) {
            if (!attendanceConnection.isConnected())
            {
                MessageBox.Show("Please connect the device first", "Error");
                return;
            }

            downloadAttendanceRecord.RunWorkerAsync();

            //AttendanceRecord aRecord = new AttendanceRecord();
            //int idwErrorCode=0;
            
            //int iGLCount = 0;
            //int iIndex = 0;

            //Cursor = Cursors.WaitCursor;
            //lvLogs.Items.Clear();
            //attendanceConnection.enableDevice(false);//disable the device
            //try 
            //{
            //    //read all the attendance records to the memory
            //    // check for documentation about this area....
            //    System.Diagnostics.Debug.WriteLine("Obtaining List");
            //    List<AttendanceRecord> attendanceRecordList = attendanceConnection.readLogData();
            //    System.Diagnostics.Debug.WriteLine("List Obtained");
            //    foreach (AttendanceRecord eachRecord in attendanceRecordList)
            //    {
            //        iGLCount++;
            //        lvLogs.Items.Add(iGLCount.ToString());
            //        lvLogs.Items[iIndex].SubItems.Add(eachRecord.SdwEnrollNumber);//modify by Darcy on Nov.26 2009
            //        lvLogs.Items[iIndex].SubItems.Add(eachRecord.IdwVerifyMode.ToString());
            //        lvLogs.Items[iIndex].SubItems.Add(eachRecord.IdwInOutMode.ToString());
            //        lvLogs.Items[iIndex].SubItems.Add(eachRecord.IdwYear.ToString() + "-" + eachRecord.IdwMonth.ToString() + "-" + eachRecord.IdwDay.ToString() + " " + eachRecord.IdwHour.ToString() + ":" + eachRecord.IdwMinute.ToString() + ":" + eachRecord.IdwSecond.ToString());
            //        lvLogs.Items[iIndex].SubItems.Add(eachRecord.IdwWorkcode.ToString());
            //        iIndex++;
            //    }
            //}
            //catch (Exception f)
            //{
            //    Cursor = Cursors.Default;
            //    attendanceConnection.getAttendance().GetLastError(ref idwErrorCode);

            //    if (idwErrorCode != 0)
            //    {
            //        MessageBox.Show("Reading data from terminal failed,ErrorCode: " + idwErrorCode.ToString(),"Error");
            //    }
            //    else
            //    {
            //        MessageBox.Show("No data from terminal returns!","Error");
            //    }
            //}
            //attendanceConnection.enableDevice(true);//enable the device
            //Cursor = Cursors.Default;
        }

        //Clear all attendance records from terminal
        private void btnClearGLog_Click(object sender, EventArgs e)
        {
            if (!attendanceConnection.isConnected())
            {
                MessageBox.Show("Please connect the device first", "Error");
                return;
            }
            int idwErrorCode = 0;

            lvLogs.Items.Clear();
            attendanceConnection.enableDevice(false);//disable the device

            try 
            {
                attendanceConnection.deleteAllRecord();
                MessageBox.Show("All att Logs have been cleared from teiminal!", "Success");
            }
            catch (Exception f)
            {
                attendanceConnection.getAttendance().GetLastError(ref idwErrorCode);
                MessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }
            attendanceConnection.enableDevice(true);//enable the device
        }

        //Get the count of attendance records in from ternimal
        private void btnGetDeviceStatus_Click(object sender, EventArgs e)
        {
            if (!attendanceConnection.isConnected())
            {
                MessageBox.Show("Please connect the device first", "Error");
                return;
            }
            int idwErrorCode = 0;
            int iValue = 0;

            attendanceConnection.enableDevice(false);//disable the device
            try //Here we use the function "GetDeviceStatus" to get the record's count.The parameter "Status" is 6.
            {
                iValue = attendanceConnection.getDeviceStatus(Connector.COUNT_ALL_ATTENDANCE_ENTRY);
                MessageBox.Show("The count of the AttLogs in the device is " + iValue.ToString(), "Success");
            }
            catch (Exception f)
            {
                attendanceConnection.getAttendance().GetLastError(ref idwErrorCode);
                MessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }
            attendanceConnection.enableDevice(true);//enable the device
        }
        #endregion

        private void enableButtons(bool status)
        {
            // Enable buttons
            btnGetDeviceStatus.Enabled = status;
            btnGetGeneralLogData.Enabled = status;
            btnClearGLog.Enabled = status;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            // Add Long Operation here
            attendanceConnection.connect(txtIP.Text, txtPort.Text);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            int idwErrorCode = 0;
            // Check connection result
            if (attendanceConnection.isConnected())
            {
                // This method is not allowed
                btnConnect.Text = "DisConnect";
                btnConnect.Refresh();
                lblState.Text = "Current State:Connected";
                //iMachineNumber = 1;//In fact,when you are using the tcp/ip communication,this parameter will be ignored,that is any integer will all right.Here we use 1.
                //attendanceConnection.getAttendance().RegEvent(iMachineNumber, 65535);//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
                enableButtons(true);
            }
            else
            {
                attendanceConnection.getAttendance().GetLastError(ref idwErrorCode);
                MessageBox.Show("Unable to connect the device,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }
            Cursor = Cursors.Default;
        }

        private void downloadAttendanceRecord_DoWork(object sender, DoWorkEventArgs e)
        {
            Invoke((MethodInvoker)(() => {
            //AttendanceRecord aRecord = new AttendanceRecord();
            int idwErrorCode = 0;

            Cursor = Cursors.WaitCursor;
            
            attendanceConnection.enableDevice(false);//disable the device
            try
            {
                //read all the attendance records to the memory
                // check for documentation about this area....
                System.Diagnostics.Debug.WriteLine("Obtaining List");
                attendanceRecordList = attendanceConnection.readLogData();
                System.Diagnostics.Debug.WriteLine("List Obtained");
                
            }
            catch (Exception f)
            {
                Cursor = Cursors.Default;
                attendanceConnection.getAttendance().GetLastError(ref idwErrorCode);

                if (idwErrorCode != 0)
                {
                    MessageBox.Show("Reading data from terminal failed,ErrorCode: " + idwErrorCode.ToString(), "Error");
                }
                else
                {
                    MessageBox.Show("No data from terminal returns!", "Error");
                }
            }
            attendanceConnection.enableDevice(true);//enable the device
            }));
        }

        private void downloadAttendanceRecord_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Invoke((MethodInvoker)(() => {
            lvLogs.Items.Clear();
            int iGLCount = 0;
            int iIndex = 0;
            foreach (AttendanceRecord eachRecord in attendanceRecordList)
            {
                iGLCount++;
                lvLogs.Items.Add(iGLCount.ToString());
                lvLogs.Items[iIndex].SubItems.Add(eachRecord.SdwEnrollNumber);//modify by Darcy on Nov.26 2009
                lvLogs.Items[iIndex].SubItems.Add(eachRecord.IdwVerifyMode.ToString());
                lvLogs.Items[iIndex].SubItems.Add(eachRecord.IdwInOutMode.ToString());
                lvLogs.Items[iIndex].SubItems.Add(eachRecord.IdwYear.ToString() + "-" + eachRecord.IdwMonth.ToString() + "-" + eachRecord.IdwDay.ToString() + " " + eachRecord.IdwHour.ToString() + ":" + eachRecord.IdwMinute.ToString() + ":" + eachRecord.IdwSecond.ToString());
                lvLogs.Items[iIndex].SubItems.Add(eachRecord.IdwWorkcode.ToString());
                iIndex++;
            }
            Cursor = Cursors.Default;
            }));
        }


   }
} 