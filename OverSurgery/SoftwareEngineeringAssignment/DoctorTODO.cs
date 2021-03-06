﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoftwareEngineeringAssignment
{
    public partial class DoctorTODO : Form
    {
        //Variables
        Form f; 
        Staff m_s; //static Details of the staff
        BusinessMetaLayer instance = BusinessMetaLayer.instance(); //Operates as the link to the database
        List<ExtentionRequest> requestList = new List<ExtentionRequest>();//stores the list of the medicines
        /// <summary>
        /// This constructer will take the staff details from the previous form and set up the rest of the Form
        /// </summary>
        /// <param name="p_s">The staff details passed from the previous form</param>
        public DoctorTODO(Staff p_s)
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            m_s = p_s;
            lblDoctor.Text = m_s.getType;
            lblName.Text = "Staff ID: " + m_s.getStaffID;
            loadPerscriptions();
        }
        private void loadPerscriptions()
        {
            requestList = instance.getExtentions();
            lvExtention.Clear();
            lvExtention.Columns.Add("ExtentionID", 100);
            lvExtention.Columns.Add("PatientID", 100);
            lvExtention.Columns.Add("FirstName", 200);
            lvExtention.Columns.Add("LastName", 200);
            lvExtention.Columns.Add("MedicineName", 200);
            lvExtention.Columns.Add("StartDate", 200);
            lvExtention.Columns.Add("EndDate", 200);
            lvExtention.Columns.Add("NewEndDate", 200);

            foreach (ExtentionRequest er in requestList)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = er.getExtentionID.ToString();
                lvi.SubItems.Add(er.getPatientID.ToString());
                lvi.SubItems.Add(er.getFirstName);
                lvi.SubItems.Add(er.getLastName);
                lvi.SubItems.Add(er.getMedicineName);
                lvi.SubItems.Add(er.getStartDate.ToShortDateString());
                lvi.SubItems.Add(er.getEndDate.ToShortDateString());
                lvi.SubItems.Add(er.getNewEndDate.ToShortDateString());
                lvExtention.Items.Add(lvi);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAcceptPrescription_Click(object sender, EventArgs e)
        {
            bool parsed = false;
            int Exid=0;
            if (txtExtentionID.Text != "")
            {
                try
                {
                    Exid = int.Parse(txtExtentionID.Text);
                    parsed = true;
                }
                catch
                {
                    MessageBox.Show("Invalid ExtensionID", "Invalid ExtensionID");
                }
                if (parsed)
                {
                    int i = 0;
                    foreach (ExtentionRequest er in requestList)
                    {
                        if (Exid == er.getExtentionID)
                        {
                            Exid = i;
                        }
                        i++;
                    }
                    int MedID = 0;
                    List<Medicine> medicineList = new List<Medicine>();
                    medicineList = instance.getAllMedicine();
                    try
                    {
                        foreach (Medicine m in medicineList)
                        {
                            if (m.getMedicineName == requestList[Exid].getMedicineName)
                            {
                                MedID = m.getMedicineID;
                            }
                        }
                        instance.ExecuteQuery("UPDATE MedicineLink SET EndDate='" + requestList[Exid].getNewEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE PatientID=" + requestList[Exid].getPatientID + " AND MedicineID=" + MedID + " AND StartDate='" + requestList[Exid].getStartDate.ToString("yyyy-MM-dd HH:mm:ss") + "' AND EndDate='" + requestList[Exid].getEndDate.ToString("yyyy-MM-dd HH:mm:ss") + "';");
                        instance.ExecuteQuery("DELETE FROM extensionrequests WHERE ExtensionID=" + txtExtentionID.Text);
                        MessageBox.Show("The prescription has been extended", "Prescription Extended");
                        loadPerscriptions();
                    }
                    catch
                    {
                        MessageBox.Show("ExtensionID does not exist","Invalid ExtensionID");
                    }
                }
            }
            else
            {
                MessageBox.Show("ExtensionID cannot be blank", "Invalid ExtensionID");
            }
        }
        private void btnDeclinePrescription_Click(object sender, EventArgs e)
        {
            bool parsed = false;
            int Exid = 0;
            if (txtExtentionID.Text != "")
            {
                try
                {
                    Exid = int.Parse(txtExtentionID.Text);
                    parsed = true;
                }
                catch
                {
                    MessageBox.Show("Invalid ExtensionID", "Invalid ExtensionID");
                }
                if (parsed)
                {
                    parsed = false;
                    foreach(ExtentionRequest er in requestList)
                    {
                        if(Exid == er.getExtentionID)
                        {
                            parsed = true;
                        }
                    }
                    if (parsed)
                    {
                        try
                        {
                            instance.ExecuteQuery("DELETE FROM extensionrequests WHERE ExtensionID=" + txtExtentionID.Text);
                            MessageBox.Show("The Extension Request has been declined", "Request Denied");
                            loadPerscriptions();
                        }
                        catch
                        {
                            MessageBox.Show("The ExtensionID was Invalid", "The ExtensionID was Invalid");
                        }
                    }
                    else
                    {
                        MessageBox.Show("extensionID does not exist", "Invalid extensionID");
                    }
                }
            }
            else
            {
                MessageBox.Show("ExtensionID cannot be blank", "Invalid ExtensionID");
            }
        }

        private void btnViewPatient_Click(object sender, EventArgs e)
        {
            List<Patient> patientList = new List<Patient>();
            patientList = instance.getPatientList();

            if (txtPatientID.Text != "")
            {
                int patientID;
                bool exists = false;
                try
                {
                    patientID = int.Parse(txtPatientID.Text);
                    foreach(Patient patient in patientList)
                    {
                        if(patient.getPatientID == patientID)
                        {
                            exists = true;
                        }
                    }
                    if (exists)
                    {
                        Patient p = instance.getPatientByID(patientID);
                        f = new PatientMenu(p, m_s);
                        f.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Patient does not exist", "No patient found");
                    }
                }
                catch
                {
                    MessageBox.Show("Invalid PatientID");
                }
            }
            else
            {
                MessageBox.Show("Please Enter a PatientID", "PatientID Missing");
            }
        }
    }
}
