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
        //public zkemkeeper.CZKEMClass axCZKEM1 = new zkemkeeper.CZKEMClass();
        private Connector attendanceConnection = new Connector();

        public AttLogsMain() {
            InitializeComponent();
        }

        /*************************************************************************************************
        * Before you refer to this demo,we strongly suggest you read the development manual deeply first.*
        * This part is for demonstrating the communication with your device.                             *
        * ************************************************************************************************/
        #region Communication
        //private bool bIsConnected = false;//the boolean value identifies whether the device is connected
        private int iMachineNumber = 1;//the serial number of the device.After connecting the device ,this value will be changed.

        //If your device supports the TCP/IP communications, you can refer to this.
        //when you are using the tcp/ip communication,you can distinguish different devices by their IP address.
        private void btnConnect_Click(object sender, EventArgs e) 
        {
            if (!Validator.checkIpAddress(txtIP.Text) || !Validator.checkPort(txtPort.Text)){
                MessageBox.Show("IP and Port cannot be null or incorrect", "Error");
                return;
            }
            int idwErrorCode = 0;

            Cursor = Cursors.WaitCursor;
            if (attendanceConnection.isConnected())
            {
                // Disconnection Procedures
                attendanceConnection.disconnect();
                //axCZKEM1.Disconnect();
                //bIsConnected = false;
                btnConnect.Text = "Connect";
                lblState.Text = "Current State:DisConnected";
                Cursor = Cursors.Default;
                return; // Leave the application immediately
            }
            // Connection procedure
            //bIsConnected = attendanceConnection.connect(txtIP.Text, txtPort.Text);
            //bIsConnected = axCZKEM1.Connect_Net(txtIP.Text, Convert.ToInt32(txtPort.Text));
            // This particular part may spend a lot of time...

            // Check connection result
            if (attendanceConnection.connect(txtIP.Text, txtPort.Text))
            {
                btnConnect.Text = "DisConnect";
                btnConnect.Refresh();
                lblState.Text = "Current State:Connected";
                iMachineNumber = 1;//In fact,when you are using the tcp/ip communication,this parameter will be ignored,that is any integer will all right.Here we use 1.
                attendanceConnection.getAttendance().RegEvent(iMachineNumber, 65535);
                //axCZKEM1.RegEvent(iMachineNumber, 65535);//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
            } else {
                attendanceConnection.getAttendance().GetLastError(ref idwErrorCode);
                MessageBox.Show("Unable to connect the device,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }
            Cursor = Cursors.Default;
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

            AttendanceRecord aRecord = new AttendanceRecord();
            int idwErrorCode=0;

            string sdwEnrollNumber = "";
            int idwVerifyMode=0;
            int idwInOutMode=0;
            int idwYear=0;
            int idwMonth=0;
            int idwDay=0;
            int idwHour=0;
            int idwMinute=0;
            int idwSecond = 0;
            int idwWorkcode = 0;
            
            int iGLCount = 0;
            int iIndex = 0;

            Cursor = Cursors.WaitCursor;
            lvLogs.Items.Clear();
            attendanceConnection.getAttendance().EnableDevice(iMachineNumber, false);//disable the device
            if (attendanceConnection.getAttendance().ReadGeneralLogData(iMachineNumber))
            {
                //read all the attendance records to the memory
                // check for documentation about this area....
                while (attendanceConnection.getAttendance().SSR_GetGeneralLogData(iMachineNumber, out sdwEnrollNumber, out idwVerifyMode,
                            out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idwWorkcode))//get records from the memory
                {
                    iGLCount++;
                    lvLogs.Items.Add(iGLCount.ToString());
                    lvLogs.Items[iIndex].SubItems.Add(sdwEnrollNumber);//modify by Darcy on Nov.26 2009
                    lvLogs.Items[iIndex].SubItems.Add(idwVerifyMode.ToString());
                    lvLogs.Items[iIndex].SubItems.Add(idwInOutMode.ToString());
                    lvLogs.Items[iIndex].SubItems.Add(idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " " + idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString());
                    lvLogs.Items[iIndex].SubItems.Add(idwWorkcode.ToString());
                    iIndex++;
                }
            }
            else
            {
                Cursor = Cursors.Default;
                attendanceConnection.getAttendance().GetLastError(ref idwErrorCode);

                if (idwErrorCode != 0)
                {
                    MessageBox.Show("Reading data from terminal failed,ErrorCode: " + idwErrorCode.ToString(),"Error");
                }
                else
                {
                    MessageBox.Show("No data from terminal returns!","Error");
                }
            }
            attendanceConnection.getAttendance().EnableDevice(iMachineNumber, true);//enable the device
            Cursor = Cursors.Default;
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
            attendanceConnection.getAttendance().EnableDevice(iMachineNumber, false);//disable the device
            if (attendanceConnection.getAttendance().ClearGLog(iMachineNumber))
            {
                attendanceConnection.getAttendance().RefreshData(iMachineNumber);//the data in the device should be refreshed
                MessageBox.Show("All att Logs have been cleared from teiminal!", "Success");
            }
            else
            {
                attendanceConnection.getAttendance().GetLastError(ref idwErrorCode);
                MessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }
            attendanceConnection.getAttendance().EnableDevice(iMachineNumber, true);//enable the device
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

            attendanceConnection.getAttendance().EnableDevice(iMachineNumber, false);//disable the device
            if (attendanceConnection.getAttendance().GetDeviceStatus(iMachineNumber, 6, ref iValue)) //Here we use the function "GetDeviceStatus" to get the record's count.The parameter "Status" is 6.
            {
                MessageBox.Show("The count of the AttLogs in the device is " + iValue.ToString(), "Success");
            }
            else
            {
                attendanceConnection.getAttendance().GetLastError(ref idwErrorCode);
                MessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }
            attendanceConnection.getAttendance().EnableDevice(iMachineNumber, true);//enable the device
        }
        #endregion

   }
} 