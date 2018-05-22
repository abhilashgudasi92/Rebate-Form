/* This program is written to manage Rebate forms. 
   It allows :
     1. Adding a new Rebate form details
     2. Modifying existing form
     3. Delete a form
     4. Display all the records in Listview at runtime
     5. Constraints are applied on some of the fields like first_name can have 25 characters, phone numbers should contain only numbers etc..
     Written by: Abhilash Gudasi - abg160130@utdallas.edu
  */

using System;
using System.IO;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Asg2_abg160130
{
    public partial class Form1 : Form
    {
        int numOfRecords = 0;       //Total number of records 
        int recordsAdded = 0;       // New Records added in present session
        int backSpaceKeyPress;      //Backspace key press count
        String databasePath = Environment.CurrentDirectory + "/" + "CS6326Asg2.txt";    //File path where database is maintained

        public Form1()
        {
            InitializeComponent();
            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker1.MaxDate = DateTime.Now;
            backSpaceKeyPress = 0;

            // Reading the text file and populating Listview
            FileStream rebateDatabase = new FileStream(databasePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamReader readDatabase = new StreamReader(rebateDatabase);
            string readline = "";
            string[] fields;
            ListViewItem listNames;

            //Filling up the listview from database file when form is loaded
            while ((readline = readDatabase.ReadLine()) != null)
            {
                numOfRecords++;
                fields = readline.Split('\t'); //Split the line.
                listNames = new ListViewItem(); //"Row" object.

                //For each item in the line.
                for (int i = 0; i < fields.Length; i++)
                {
                    if (i == 0)
                    {
                        listNames.Text = fields[i];
                    }
                    else
                    {
                        listNames.SubItems.Add(fields[i]);
                    }
                }

                rebateItems.Items.Add(listNames);
            }

            rebateDatabase.Close();
            toolStripStatusLabel1.Text = "Total rebate records: " + numOfRecords.ToString();
            toolStripStatusLabelError.Text = "Running";
            toolStripStatusLabelError.ForeColor = Color.DarkGreen;
            statusStrip1.Refresh();
        }


        String formStartTime;// = System.DateTime.Now.ToString("HH:MM:" + DateTime.Now.Second);
        private void saveButton_Click(object sender, EventArgs e)
        {
            var formEndTime = DateTime.Now.ToString("HH:MM:" + DateTime.Now.Second);

            //Check for file presence, if not create one
            if (!File.Exists(databasePath))
            {
                File.CreateText(databasePath);
            }

            duplicateCheckSave(formEndTime);       //Check for record duplicates while adding new record
        }

        //Deleting the selected record
        private void deleteButton_Click(object sender, EventArgs e)
        {
            int itemSelected = rebateItems.SelectedIndices[0];
            StringBuilder sb = new StringBuilder("");
            if (File.Exists(databasePath))
            {
                FileStream readDatabase = new FileStream(databasePath, FileMode.Open, FileAccess.Read);     //Reading the database file
                using (StreamReader sr = new StreamReader(readDatabase))
                {
                    string readLine;
                    string[] items;
                    Boolean recordPresent = false;
                    string firstName = rebateItems.Items[itemSelected].SubItems[0].Text.Trim();
                    string phoneNumber = rebateItems.Items[itemSelected].SubItems[9].Text.Trim();
                    string lastName = rebateItems.Items[itemSelected].SubItems[2].Text.Trim();

                    while ((readLine = sr.ReadLine()) != null)
                    {
                        items = readLine.Split('\t');       //Split the line with '\t' delimiter.

                        //For each item in the line.
                        Boolean result = ((string.Equals(items[0], firstName,
                            StringComparison.OrdinalIgnoreCase) && (string.Equals(items[9],
                            phoneNumber, StringComparison.OrdinalIgnoreCase)) &&
                            (string.Equals(items[2].ToString(), lastName, StringComparison.OrdinalIgnoreCase))));

                        // If a record with same name exists, delete the record
                        if (result == true)
                        {
                            recordPresent = true;
                            continue;
                        }
                        sb.AppendLine(readLine);
                    }
                }
                readDatabase.Close();
            }

            File.Delete(databasePath);
            //Check for file presence, if not create one
            if (!File.Exists(databasePath))
            {
                var fs = File.CreateText(databasePath);
                fs.Close();
            }
            using (FileStream readDatabase = new FileStream(databasePath, FileMode.Open, FileAccess.Write, FileShare.Write))
            {
                StreamWriter sw = new StreamWriter(readDatabase);
                sw.BaseStream.Seek(0, SeekOrigin.Begin);
                sw.WriteLine(sb);
                sw.Flush();
                sw.Close();
                readDatabase.Close();
            }

            rebateItems.Items.RemoveAt(itemSelected);               //To delete record on User Interface

            txtFirstName.Clear();
            txtMiddleInitial.Clear();
            txtLastName.Clear();
            txtAddressLine1.Clear();
            txtAddressLine2.Clear();
            txtCity.Clear();
            txtState.Clear();
            txtEmailAddress.Clear();
            txtZipCode.Clear();
            txtPhoneNumber.Clear();
            comboBoxGender.ResetText();
            comboBoxGender.SelectedIndex = -1;
            checkBoxYes.Select();
            dateTimePicker1.Text = DateTime.Now.ToShortDateString();
            txtFirstName.Focus();

            numOfRecords--;
            toolStripStatusLabel1.Text = "Total rebate records: " + numOfRecords.ToString();
            toolStripStatusLabelError.Text = "*Record deleted successfully";
            toolStripStatusLabelError.ForeColor = Color.Red;
            statusStrip1.Refresh();
        }

        //Function to check if any record being added is dupliacte
        private void duplicateCheckSave(String formEndTime)
        {
            Boolean recordPresent = false; 
            string[] items;
            FileStream readDatabase = new FileStream(databasePath, FileMode.Open, FileAccess.Read);
            using (StreamReader sr = new StreamReader(readDatabase))
            {
                string readLine;
                string firstName = txtFirstName.Text.Trim();
                string phoneNumber = txtPhoneNumber.Text.Trim();
                string lastName = txtLastName.Text.Trim();
                while ((readLine = sr.ReadLine()) != null)
                {
                    items = readLine.Split('\t');       //Split the line with '\t' delimiter.

                    //For each item in the line.
                    Boolean result = ((string.Equals(items[0], firstName, 
                        StringComparison.OrdinalIgnoreCase) && (string.Equals(items[9],
                        phoneNumber, StringComparison.OrdinalIgnoreCase)) && 
                        (string.Equals(items[2].ToString(), lastName, StringComparison.OrdinalIgnoreCase))));

                    // If a record with same name exists, show error
                    if (result == true)
                    {
                        recordPresent = true;
                        txtFirstName.BackColor = Color.LightSalmon;
                        txtLastName.BackColor = Color.LightSalmon;
                        txtPhoneNumber.BackColor = Color.LightSalmon;
                        toolStripStatusLabelError.Text = "*A record already exist with same name";
                        toolStripStatusLabelError.ForeColor = Color.Red;
                        statusStrip1.Refresh();
                        break;
                    }
                }
            }   
            // If input values are unique add the record to database file and listview
            if (recordPresent == false)
            {
                Int64 phone;
                // validation of phone number
                if ((!(long.TryParse(txtPhoneNumber.Text, out phone))))
                {
                    txtFirstName.BackColor = Color.White;
                    txtLastName.BackColor = Color.White;
                    txtMiddleInitial.BackColor = Color.White;
                    txtPhoneNumber.BackColor = Color.LightSalmon;
                    toolStripStatusLabelError.Text = "*Invalid Phone number";
                    toolStripStatusLabelError.ForeColor = Color.Red;
                    statusStrip1.Refresh();
                }

                // Add record to textfile and Listview
                else
                {
                    ListViewItem listItem = rebateItems.Items.Add(txtFirstName.Text);
                    listItem.SubItems.Add(txtMiddleInitial.Text);
                    listItem.SubItems.Add(txtLastName.Text);
                    listItem.SubItems.Add(txtAddressLine1.Text);
                    listItem.SubItems.Add(txtAddressLine2.Text);
                    listItem.SubItems.Add(txtCity.Text);
                    listItem.SubItems.Add(txtState.Text);
                    listItem.SubItems.Add(txtZipCode.Text);
                    listItem.SubItems.Add(comboBoxGender.Text);
                    listItem.SubItems.Add(txtPhoneNumber.Text);
                    listItem.SubItems.Add(txtEmailAddress.Text);
                    String proofSubmitted = checkBoxYes.Checked ? "Yes":"No";
                    listItem.SubItems.Add(proofSubmitted);
                    listItem.SubItems.Add(dateTimePicker1.Text);

                    StringBuilder recordLine = new StringBuilder();
                    recordLine.AppendLine(txtFirstName.Text + "\t" + txtMiddleInitial.Text + "\t" + txtLastName.Text + 
                        "\t" + txtAddressLine1.Text + "\t" + txtAddressLine2.Text + "\t" + txtCity.Text + "\t" + 
                        txtState.Text + "\t" + txtZipCode.Text + "\t" + comboBoxGender.Text + "\t" + txtPhoneNumber.Text + "\t" + 
                        txtEmailAddress.Text + "\t" + proofSubmitted + "\t" + dateTimePicker1.Text + "\t" + formStartTime + "\t" + 
                        formEndTime + "\t" + backSpaceKeyPress);

                    var formDatabase = new FileStream(databasePath, FileMode.Append);
                    var writer = new StreamWriter(formDatabase);
                    writer.Write(recordLine.ToString());
                    writer.Flush();
                    writer.Close();

                    txtFirstName.BackColor = Color.White;
                    txtLastName.BackColor = Color.White;
                    txtMiddleInitial.BackColor = Color.White;
                    txtPhoneNumber.BackColor = Color.White;
                    toolStripStatusLabelError.ForeColor = Color.Black;
                    txtFirstName.Clear();
                    txtMiddleInitial.Clear();
                    txtLastName.Clear();
                    txtAddressLine1.Clear();
                    txtAddressLine2.Clear();
                    txtCity.Clear();
                    txtState.Clear();
                    txtEmailAddress.Clear();
                    txtZipCode.Clear();
                    txtPhoneNumber.Clear();
                    comboBoxGender.ResetText();
                    comboBoxGender.SelectedIndex = -1;
                    dateTimePicker1.Text = DateTime.Now.ToShortDateString();
                    txtFirstName.Focus();
                    //formStartTime = System.DateTime.Now.ToString("HH:MM:" + DateTime.Now.Second);
                    numOfRecords++;
                    recordsAdded++;
                    backSpaceKeyPress = 0;
                    toolStripStatusLabel1.Text = "Total Records: " + numOfRecords.ToString();
                    toolStripStatusLabelError.Text = "Record added successfully";
                    toolStripStatusLabelError.ForeColor = Color.Green;
                    statusStrip1.Refresh();
                }
            }
        }

        // Populate data from selected listItems in Texboxes
        private void rebateItems_MouseClick(object sender, MouseEventArgs e)
        {
            updateButton.Enabled = true;
            deleteButton.Enabled = true;
            saveButton.Enabled = false;
            toolStripStatusLabelError.Text = "Record selected";
            toolStripStatusLabelError.ForeColor = Color.DarkGreen;
            txtFirstName.Text = rebateItems.SelectedItems[0].SubItems[0].Text;
            txtMiddleInitial.Text = rebateItems.SelectedItems[0].SubItems[1].Text;
            txtLastName.Text = rebateItems.SelectedItems[0].SubItems[2].Text;
            txtAddressLine1.Text = rebateItems.SelectedItems[0].SubItems[3].Text;
            txtAddressLine2.Text = rebateItems.SelectedItems[0].SubItems[4].Text;
            txtCity.Text = rebateItems.SelectedItems[0].SubItems[5].Text;
            txtState.Text = rebateItems.SelectedItems[0].SubItems[6].Text;
            txtZipCode.Text = rebateItems.SelectedItems[0].SubItems[7].Text;
            comboBoxGender.Text = rebateItems.SelectedItems[0].SubItems[8].Text;
            txtPhoneNumber.Text = rebateItems.SelectedItems[0].SubItems[9].Text;
            txtEmailAddress.Text = rebateItems.SelectedItems[0].SubItems[10].Text;

            if (rebateItems.SelectedItems[0].SubItems[11].Text == "Yes")
                checkBoxYes.Checked = true;
            else if (rebateItems.SelectedItems[0].SubItems[11].Text == "No")
                checkBoxNo.Checked = true;

            dateTimePicker1.Text = rebateItems.SelectedItems[0].SubItems[12].Text;
        }

        //Updating the selected record
        private void updateButton_Click(object sender, EventArgs e)
        {
            if (rebateItems.SelectedItems == null)
            {
                toolStripStatusLabelError.Text = "Need to select a rebate item to update!!!";
                toolStripStatusLabelError.ForeColor = Color.Red;
                return;
            }
            String formModifyTime = System.DateTime.Now.ToString("HH:MM:" + DateTime.Now.Second);
            Boolean recordPresent = false;
            string[] items;
            FileStream readDatabase = new FileStream(databasePath, FileMode.Open, FileAccess.Read);
            using (StreamReader sr = new StreamReader(readDatabase))
            {
                string readLine;
                string firstName = txtFirstName.Text;
                string phoneNumber = txtPhoneNumber.Text;
                string lastName = txtLastName.Text;
                while ((readLine = sr.ReadLine()) != null)
                {
                    items = readLine.Split('\t');       //Split the line with '\t' delimiter.

                    //For each item in the line.
                    Boolean result = ((string.Equals(items[0], firstName,
                        StringComparison.OrdinalIgnoreCase) && (string.Equals(items[9],
                        phoneNumber, StringComparison.OrdinalIgnoreCase)) &&
                        (string.Equals(items[2].ToString(), lastName, StringComparison.OrdinalIgnoreCase))));
                    Boolean selected = (items[0] == rebateItems.FocusedItem.SubItems[0].Text && 
                        items[9] == rebateItems.FocusedItem.SubItems[9].Text && 
                        items[2] == rebateItems.FocusedItem.SubItems[2].Text);

                    // If a record with same name exists, show error
                    if (result == true && selected == false)
                    {
                        recordPresent = true;
                        txtFirstName.BackColor = Color.LightSalmon;
                        txtLastName.BackColor = Color.LightSalmon;
                        txtMiddleInitial.BackColor = Color.LightSalmon;
                        toolStripStatusLabelError.Text = "*A record already exist";
                        toolStripStatusLabelError.ForeColor = Color.Red;
                        statusStrip1.Refresh();
                        break;
                    }
                }
            }
          
            if (recordPresent == false)
            {
                Int64 phone;
                // Phone number is valid or not check
                if ((!(long.TryParse(txtPhoneNumber.Text, out phone))))
                {
                    txtFirstName.BackColor = Color.White;
                    txtLastName.BackColor = Color.White;
                    txtMiddleInitial.BackColor = Color.White;
                    txtPhoneNumber.BackColor = Color.LightSalmon;
                    toolStripStatusLabelError.Text = "*Invalid Phone number";
                    toolStripStatusLabelError.ForeColor = Color.Red;
                    statusStrip1.Refresh();
                }

                // Add updated record to textfile and Listview
                else
                {
                    String recordLine;
                    string tempFile = Path.GetTempFileName();
                    String proofSubmitted = checkBoxYes.Checked ? "Yes" : "No";
                    String tab = "\t";

                    recordLine = txtFirstName.Text + tab + txtMiddleInitial.Text + tab + txtLastName.Text +
                        tab + txtAddressLine1.Text + tab + txtAddressLine2.Text + tab + txtCity.Text + tab +
                        txtState.Text + tab + txtZipCode.Text + tab + comboBoxGender.Text + tab + txtPhoneNumber.Text + tab +
                        txtEmailAddress.Text + tab + proofSubmitted + tab + dateTimePicker1.Text + tab + formStartTime + tab + 
                        formModifyTime + tab + backSpaceKeyPress;

                    var formDatabase = new FileStream(databasePath, FileMode.Open,FileAccess.ReadWrite);
                    StreamReader read = new StreamReader(formDatabase);
                    using (StreamWriter writer = new StreamWriter(tempFile))
                    {
                        String readLine;
                        while((readLine = read.ReadLine()) != null)
                        {
                            items = readLine.Split('\t');

                            if (!(items[0] == rebateItems.FocusedItem.SubItems[0].Text && 
                                items[9] == rebateItems.FocusedItem.SubItems[9].Text && 
                                items[2] == rebateItems.FocusedItem.SubItems[2].Text))
                            {
                                writer.WriteLine(readLine);
                            }
                            else
                            {
                                writer.WriteLine(recordLine);      //Replacing the record
                            }
                        }
                        writer.Flush();
                        writer.Close();
                    }
                    formDatabase.Close();
                    File.Delete(databasePath);
                    File.Move(tempFile, databasePath);

                    //Updating the listview of the form for user
                    rebateItems.SelectedItems[0].SubItems[0].Text = txtFirstName.Text;
                    rebateItems.SelectedItems[0].SubItems[1].Text = txtMiddleInitial.Text;
                    rebateItems.SelectedItems[0].SubItems[2].Text = txtLastName.Text;
                    rebateItems.SelectedItems[0].SubItems[3].Text = txtAddressLine1.Text;
                    rebateItems.SelectedItems[0].SubItems[4].Text = txtAddressLine2.Text;
                    rebateItems.SelectedItems[0].SubItems[5].Text = txtCity.Text;
                    rebateItems.SelectedItems[0].SubItems[6].Text = txtState.Text;
                    rebateItems.SelectedItems[0].SubItems[7].Text = txtZipCode.Text;
                    rebateItems.SelectedItems[0].SubItems[8].Text = comboBoxGender.Text;
                    rebateItems.SelectedItems[0].SubItems[9].Text = txtPhoneNumber.Text;
                    rebateItems.SelectedItems[0].SubItems[10].Text = txtEmailAddress.Text;
                    proofSubmitted = checkBoxYes.Checked ? "Yes" : "No";
                    rebateItems.SelectedItems[0].SubItems[11].Text = proofSubmitted;
                    rebateItems.SelectedItems[0].SubItems[12].Text = dateTimePicker1.Text; 

                    txtFirstName.BackColor = Color.White;
                    txtLastName.BackColor = Color.White;
                    txtMiddleInitial.BackColor = Color.White;
                    txtPhoneNumber.BackColor = Color.White;
                    toolStripStatusLabelError.ForeColor = Color.Black;
                    txtFirstName.Clear();
                    txtMiddleInitial.Clear();
                    txtLastName.Clear();
                    txtAddressLine1.Clear();
                    txtAddressLine2.Clear();
                    txtCity.Clear();
                    txtState.Clear();
                    txtEmailAddress.Clear();
                    txtZipCode.Clear();
                    txtPhoneNumber.Clear();
                    comboBoxGender.ResetText();
                    comboBoxGender.SelectedIndex = -1;
                    dateTimePicker1.Text = DateTime.Now.ToShortDateString();
                    txtFirstName.Focus();

                    backSpaceKeyPress = 0;
                    toolStripStatusLabel1.Text = "Total Records: " + numOfRecords.ToString();
                    toolStripStatusLabelError.Text = "Record modified successfully";
                    toolStripStatusLabelError.ForeColor = Color.Green;
                    statusStrip1.Refresh();
                }
            }
        }
        private void txtPhoneNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                toolStripStatusLabelError.Text = "Please enter only numeric values";
                toolStripStatusLabelError.ForeColor = Color.Red;
                statusStrip1.Refresh();
            }
            if (e.KeyChar == (char)Keys.Back)
            {
                backSpaceKeyPress++;
            }
        }

        private void txtFirstName_TextChanged(object sender, EventArgs e)
        {
            formStartTime = DateTime.Now.ToString("HH:MM:" + DateTime.Now.Second);
            toolStripStatusLabelError.ForeColor = Color.White;
        }

        private void saveButton_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                saveButton.PerformClick();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            MessageBox.Show(recordsAdded + " new records added");
        }

        private void checkBoxYes_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxYes.Checked)
                checkBoxNo.Checked = false;

            saveButton.Enabled = txtFirstName.TextLength > 0 && txtLastName.TextLength > 0 && txtAddressLine1.TextLength > 0
                && txtCity.TextLength > 0 && txtState.TextLength > 0 && txtZipCode.TextLength > 0 &&
                comboBoxGender.Text.Length > 0 && txtPhoneNumber.TextLength > 0 && txtEmailAddress.TextLength > 0;
        }

        private void checkBoxNo_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxNo.Checked)
                checkBoxYes.Checked = false;

            saveButton.Enabled = txtFirstName.TextLength > 0 && txtLastName.TextLength > 0 && txtAddressLine1.TextLength > 0
                && txtCity.TextLength > 0 && txtState.TextLength > 0 && txtZipCode.TextLength > 0 &&
                comboBoxGender.Text.Length > 0 && txtPhoneNumber.TextLength > 0 && txtEmailAddress.TextLength > 0;
        }

        private void txtZipCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                if(!e.KeyChar.Equals('-'))
                {
                    e.Handled = true;
                    txtZipCode.Clear();
                    toolStripStatusLabelError.Text = "Zip code can contain only Numerical and -";
                    toolStripStatusLabelError.ForeColor = Color.Red;
                    statusStrip1.Refresh();
                }
            }
            if (e.KeyChar == (char)Keys.Back)
            {
                backSpaceKeyPress++;
            }
        }

        private void txtEmailAddress_Leave(object sender, EventArgs e)
        {
            //if (!regex.IsMatch(txtEmailAddress.ToString()))
            if(! ((txtEmailAddress.ToString().Contains("@")) && (txtEmailAddress.ToString().Contains("."))))
            {
                txtEmailAddress.Clear();
                toolStripStatusLabelError.Text = "Email address should contain AlphaNumeric, @ and .";
                toolStripStatusLabelError.ForeColor = Color.Red;
                statusStrip1.Refresh();
            }
        }

        private void txtFirstName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back)
            {
                backSpaceKeyPress++;
            }
        }

        private void txtMiddleInitial_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back)
            {
                backSpaceKeyPress++;
            }
        }

        private void txtLastName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back)
            {
                backSpaceKeyPress++;
            }
        }

        private void txtAddressLine1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back)
            {
                backSpaceKeyPress++;
            }
        }

        private void txtAddressLine2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back)
            {
                backSpaceKeyPress++;
            }
        }

        private void txtCity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back)
            {
                backSpaceKeyPress++;
            }
        }

        private void txtState_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back)
            {
                backSpaceKeyPress++;
            }
        }

        private void txtEmailAddress_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back)
            {
                backSpaceKeyPress++;
            }
        }
    }
}
