﻿using System;
using System.Data;
using DVLDBusinessLayer;
using System.Windows.Forms;
using DVLDBusinessLayer.Tests;
using DVLD_WindowsForm.Tests.MainTests.Test;

namespace DVLD_WindowsForm.Tests.VisionTest
{
    public partial class ManageVisionTestForm : Form
    {
        private clsLocalDLApplication _DLApplication;
        private int _DLApplicationID;
        public static DataTable dtTestAppointments;
        public static DataView dtTestAppointmentsView;
        int TestTypeID = (int)GlobalEnums.enTestType.VissionTest;

        //-----------------------------------
        //---------Delegation----------------
        //-----------------------------------
        public delegate void ApplicationTestAddedEventHandler(object sender, EventArgs e);
        public event ApplicationTestAddedEventHandler ApplicationTestAdded;

        public ManageVisionTestForm(int DLApplicationID)
        {
            InitializeComponent();
            clsLocalDLApplication DLApplication = clsLocalDLApplication.Find(DLApplicationID);

            if (DLApplication == null)
                return;
            _DLApplication = DLApplication;
            _DLApplicationID = _DLApplication.LocaLDLApplicationID;

            ucApplicationInfo.FillTestAppWithDate(DLApplication);
            ucdlApplicationInfo.FillTestAppWithDate(DLApplication);

            _RefreshTestAppointmentsList();
        }

        private void ManageVisionTestForm_Load(object sender, EventArgs e)
        {
            dgvAppointmentsList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        }

        private void pbAddNewAppointment_Click(object sender, EventArgs e)
        {
            // if    hasActiveTestAppointment() show error message
            // else  open form to add new
            if (clsTestAppointment.HasActiveTestAppoinment(_DLApplicationID, TestTypeID))
            {
                MessageBox.Show("You Can Not Add New TestAppoinment when you have Active one");
                return;
            }

            AddEditTestAppointment AddEditTestFrm = new AddEditTestAppointment(_DLApplicationID, -1, TestTypeID);
            AddEditTestFrm.TestAppointmentAdded += TestApp_TesAppointmenttAdded;
            AddEditTestFrm.Show();
            _RefreshTestAppointmentsList();
        }

        private void _RefreshTestAppointmentsList()
        {
            dtTestAppointments = clsTestAppointment.GetAllTestAppointments(_DLApplication.LocaLDLApplicationID, TestTypeID); // TestTypeID
            if (dtTestAppointments == null)
                return;

            dtTestAppointmentsView = new DataView(dtTestAppointments);
            dgvAppointmentsList.DataSource = dtTestAppointmentsView;

            lbAppointmentCount.Text = dgvAppointmentsList.RowCount.ToString();
        }

        private void cmEdit_Click(object sender, EventArgs e)
        {
            int TestAppointmentID = (int)dgvAppointmentsList.CurrentRow.Cells[0].Value;

            if (clsTestAppointment.IsLockedTest(TestAppointmentID))
            {
                MessageBox.Show("You Can Not Edit this Locked TestAppointment ");
                return;
            }

            AddEditTestAppointment AddEditTestFrm = new AddEditTestAppointment(_DLApplicationID, TestAppointmentID, TestTypeID);
            AddEditTestFrm.TestAppointmentAdded += TestApp_TesAppointmenttAdded;
            AddEditTestFrm.Show();
            _RefreshTestAppointmentsList();
        }

        private void cmTakeTest_Click(object sender, EventArgs e)
        {
            int TestAppointmentID = (int)dgvAppointmentsList.CurrentRow.Cells[0].Value;
            
            if (clsTestAppointment.IsLockedTest(TestAppointmentID))
            {
                MessageBox.Show("You Can Not Take Test (this TestAppointment is Locked) ");
                return;
            }
            
            if(clsTest.IsExists(TestAppointmentID,(int)GlobalEnums.enTestResult.Passed))
            {
                MessageBox.Show("You Can Not Take Test (You Already pass this Test) ");
                return;
            }
            TakeTestForm TakeTestFrm = new TakeTestForm(_DLApplicationID, TestAppointmentID, TestTypeID);
            TakeTestFrm.TestAdded += Test_TestAdded; ;
            TakeTestFrm.Show();
            _RefreshTestAppointmentsList();
        }
        private void Test_TestAdded(object sender, EventArgs e)
        {
            _RefreshTestAppointmentsList();
        }
        private void TestApp_TesAppointmenttAdded(object sender, EventArgs e)
        {
            _RefreshTestAppointmentsList();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            ApplicationTestAdded?.Invoke(this, e);
            this.Close();
        }
    }
}
