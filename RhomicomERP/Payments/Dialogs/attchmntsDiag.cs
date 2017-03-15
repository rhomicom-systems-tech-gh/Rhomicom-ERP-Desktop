using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using InternalPayments.Classes;

namespace InternalPayments.Forms
{
  public partial class attchmntsDiag : Form
  {
    public attchmntsDiag()
    {
      InitializeComponent();
    }
    cadmaFunctions.NavFuncs myNav = new cadmaFunctions.NavFuncs();
    private long totl_vals = 0;
    private long cur_vals_idx = 0;
    private string vwSQLStmnt = "";
    private bool is_last_val = false;
    bool obeyEvnts = false;
    long last_vals_num = 0;
    //public long prmKeyID = -1;

    public bool isPrchSng = false;
    public long prmKeyID = -1;
    public int attchCtgry = 0;
    public string fldrNm = "";
    public int fldrTyp = 0;

    private void attchmntsDiag_Load(object sender, EventArgs e)
    {
      Color[] clrs = Global3.mnFrm.cmCde1.getColors();
      this.BackColor = clrs[0];
      this.loadValPanel();
    }


    private void loadValPanel()
    {
      this.obeyEvnts = false;
      if (this.searchInComboBox.SelectedIndex < 0)
      {
        this.searchInComboBox.SelectedIndex = 0;
      }
      int dsply = 0;
      if (this.dsplySizeComboBox.Text == ""
       || int.TryParse(this.dsplySizeComboBox.Text, out dsply) == false)
      {
        this.dsplySizeComboBox.Text = Global3.mnFrm.cmCde1.get_CurPlcy_Mx_Dsply_Recs().ToString();
      }

      if (this.searchForTextBox.Text == "")
      {
        this.searchForTextBox.Text = "%";
      }
      this.is_last_val = false;
      this.totl_vals = Global3.mnFrm.cmCde1.Big_Val;
      this.getValPnlData();
      this.obeyEvnts = true;
    }

    private void getValPnlData()
    {
      this.updtValTotals();
      this.populateValGridVw();
      this.updtValNavLabels();
    }

    private void updtValTotals()
    {
      this.myNav.FindNavigationIndices(int.Parse(this.dsplySizeComboBox.Text),
      this.totl_vals);

      if (this.cur_vals_idx >= this.myNav.totalGroups)
      {
        this.cur_vals_idx = this.myNav.totalGroups - 1;
      }
      if (this.cur_vals_idx < 0)
      {
        this.cur_vals_idx = 0;
      }
      this.myNav.currentNavigationIndex = this.cur_vals_idx;
    }

    private void updtValNavLabels()
    {
      this.moveFirstButton.Enabled = this.myNav.moveFirstBtnStatus();
      this.movePreviousButton.Enabled = this.myNav.movePrevBtnStatus();
      this.moveNextButton.Enabled = this.myNav.moveNextBtnStatus();
      this.moveLastButton.Enabled = this.myNav.moveLastBtnStatus();
      this.positionTextBox.Text = this.myNav.displayedRecordsNumbers();
      if (this.is_last_val == true ||
        this.totl_vals != Global3.mnFrm.cmCde1.Big_Val)
      {
        this.totalRecLabel.Text = this.myNav.totalRecordsLabel();
      }
      else
      {
        this.totalRecLabel.Text = "of Total";
      }
    }

    private void populateValGridVw()
    {
      this.obeyEvnts = false;
      DataSet dtst;
      if (this.attchCtgry == 3)
      {
        dtst = Global3.get_Pybls_Attachments(this.searchForTextBox.Text,
      this.searchInComboBox.Text, this.cur_vals_idx,
      int.Parse(this.dsplySizeComboBox.Text), this.prmKeyID, ref this.vwSQLStmnt);
      }
      else if (this.isPrchSng == false)
      {
        dtst = Global3.get_Attachments(this.searchForTextBox.Text,
      this.searchInComboBox.Text, this.cur_vals_idx,
      int.Parse(this.dsplySizeComboBox.Text), this.prmKeyID, ref this.vwSQLStmnt);
      }
      else
      {
        dtst = Global3.get_P_Attachments(
          this.searchForTextBox.Text,
 this.searchInComboBox.Text, this.cur_vals_idx,
 int.Parse(this.dsplySizeComboBox.Text),
 this.prmKeyID, ref this.vwSQLStmnt);
      }

      this.attchmntsListView.Items.Clear();
      for (int i = 0; i < dtst.Tables[0].Rows.Count; i++)
      {
        this.last_vals_num = this.myNav.startIndex() + i;
        ListViewItem nwItem = new ListViewItem(new string[] {
    (this.myNav.startIndex() + i).ToString(),
    dtst.Tables[0].Rows[i][2].ToString(),dtst.Tables[0].Rows[i][3].ToString(),
    dtst.Tables[0].Rows[i][0].ToString()});
        this.attchmntsListView.Items.Add(nwItem);
      }
      this.correctValsNavLbls(dtst);
      if (this.attchmntsListView.Items.Count > 0)
      {
        this.obeyEvnts = true;
        this.attchmntsListView.Items[0].Selected = true;
      }
      this.obeyEvnts = true;
    }

    private void correctValsNavLbls(DataSet dtst)
    {
      long totlRecs = dtst.Tables[0].Rows.Count;
      if (this.cur_vals_idx == 0 && totlRecs == 0)
      {
        this.is_last_val = true;
        this.totl_vals = 0;
        this.last_vals_num = 0;
        this.cur_vals_idx = 0;
        this.updtValTotals();
        this.updtValNavLabels();
      }
      else if (this.totl_vals == Global3.mnFrm.cmCde1.Big_Val
  && totlRecs < long.Parse(this.dsplySizeComboBox.Text))
      {
        this.totl_vals = this.last_vals_num;
        if (totlRecs == 0)
        {
          this.cur_vals_idx -= 1;
          this.updtValTotals();
          this.populateValGridVw();
        }
        else
        {
          this.updtValTotals();
        }
      }
    }

    private void valPnlNavButtons(object sender, System.EventArgs e)
    {
      System.Windows.Forms.ToolStripButton sentObj =
        (System.Windows.Forms.ToolStripButton)sender;
      this.totalRecLabel.Text = "";
      if (sentObj.Name.ToLower().Contains("first"))
      {
        this.cur_vals_idx = 0;
      }
      else if (sentObj.Name.ToLower().Contains("previous"))
      {
        this.cur_vals_idx -= 1;
      }
      else if (sentObj.Name.ToLower().Contains("next"))
      {
        this.cur_vals_idx += 1;
      }
      else if (sentObj.Name.ToLower().Contains("last"))
      {
        if (this.attchCtgry == 3)
        {
          this.totl_vals = Global3.get_Total_Pybls_Attachments(
            this.searchForTextBox.Text, this.searchInComboBox.Text,
            this.prmKeyID);
        }
        else if (this.isPrchSng == false)
        {
          this.totl_vals = Global3.get_Total_Attachments(
                    this.searchForTextBox.Text, this.searchInComboBox.Text,
                    this.prmKeyID);
        }
        else
        {
          this.totl_vals = Global3.get_Total_P_Attachments(
             this.searchForTextBox.Text, this.searchInComboBox.Text,
             this.prmKeyID);
        }

        this.is_last_val = true;
        this.updtValTotals();
        this.cur_vals_idx = this.myNav.totalGroups - 1;
      }
      this.getValPnlData();
    }

    private void okButton_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void attchmntsListView_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (this.attchmntsListView.SelectedItems.Count > 0 && this.obeyEvnts == true)
      {
        if (this.attchCtgry == 3)
        {
          Global3.mnFrm.cmCde1.getDBImageFile(
           this.attchmntsListView.SelectedItems[0].SubItems[2].Text,
           this.fldrNm, ref this.prvwPictureBox);
        }
        else if (this.isPrchSng == false)
        {
          Global3.mnFrm.cmCde1.getDBImageFile(
           this.attchmntsListView.SelectedItems[0].SubItems[2].Text,
           Global3.mnFrm.cmCde1.getSalesImgsDrctry(), ref this.prvwPictureBox);
        }
        else
        {
          Global3.mnFrm.cmCde1.getDBImageFile(
    this.attchmntsListView.SelectedItems[0].SubItems[2].Text,
    Global3.mnFrm.cmCde1.getPrchsImgsDrctry(), ref this.prvwPictureBox);
        }
      }
    }

    private void gotoButton_Click(object sender, EventArgs e)
    {
      this.loadValPanel();
    }

    private void exptExclTSrchMenuItem_Click(object sender, EventArgs e)
    {
      Global3.mnFrm.cmCde1.exprtToExcel(this.attchmntsListView);
    }

    private void positionTextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
    {
      EventArgs ex = new EventArgs();
      if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Up)
      {
        this.valPnlNavButtons(this.movePreviousButton, ex);
      }
      else if (e.KeyCode == Keys.Right || e.KeyCode == Keys.Down)
      {
        this.valPnlNavButtons(this.moveNextButton, ex);
      }
    }

    private void searchForTextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
    {
      EventArgs ex = new EventArgs();
      if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
      {
        this.gotoButton_Click(this.gotoButton, ex);
      }
    }

    private void vwSQLButton_Click(object sender, EventArgs e)
    {
      Global3.mnFrm.cmCde1.showSQL(this.vwSQLStmnt, 10);
    }

    private void rfrshTsrchMenuItem_Click(object sender, EventArgs e)
    {
      this.gotoButton_Click(this.gotoButton, e);
    }

    private void rcHstryTsrchMenuItem_Click(object sender, EventArgs e)
    {
      if (this.attchmntsListView.SelectedItems.Count <= 0)
      {
        Global3.mnFrm.cmCde1.showMsg("Please select a Transaction First!", 0);
        return;
      }
      if (this.attchCtgry == 3)
      {
        Global3.mnFrm.cmCde1.showRecHstry(Global3.mnFrm.cmCde1.get_Gnrl_Rec_Hstry(long.Parse(
               this.attchmntsListView.SelectedItems[0].SubItems[3].Text),
               "accb.accb_pybl_doc_attchmnts", "attchmnt_id"), 9);
      }
      else if (this.isPrchSng == false)
      {
        Global3.mnFrm.cmCde1.showRecHstry(Global3.mnFrm.cmCde1.get_Gnrl_Rec_Hstry(long.Parse(
               this.attchmntsListView.SelectedItems[0].SubItems[3].Text),
               "scm.scm_sales_doc_attchmnts", "attchmnt_id"), 9);
      }
      else
      {
        Global3.mnFrm.cmCde1.showRecHstry(Global3.mnFrm.cmCde1.get_Gnrl_Rec_Hstry(long.Parse(
          this.attchmntsListView.SelectedItems[0].SubItems[3].Text),
          "scm.scm_prchs_doc_attchmnts", "attchmnt_id"), 9);
      }
    }

    private void vwSQLTsrchMenuItem_Click(object sender, EventArgs e)
    {
      this.vwSQLButton_Click(this.vwSQLButton, e);
    }

    private void addButton_Click(object sender, EventArgs e)
    {
      addAttchmntDiag nwDiag = new addAttchmntDiag();
      nwDiag.attchmntIDTextBox.Text = "-1";
      nwDiag.batchID = this.prmKeyID;
      nwDiag.isPrchSng = this.isPrchSng;
      nwDiag.attchCtgry = this.attchCtgry;

      DialogResult dgrs = nwDiag.ShowDialog();
      if (dgrs == DialogResult.OK)
      {
        string tblNm = "";
        string pkNm = "";
        if (Global3.mnFrm.cmCde1.myComputer.FileSystem.FileExists(
          nwDiag.fileNmTextBox.Text) == true)
        {
          string extnsn = Global3.mnFrm.cmCde1.myComputer.FileSystem.GetFileInfo(nwDiag.fileNmTextBox.Text).Extension;
          if (this.attchCtgry == 3)
          {
            tblNm = "accb.accb_pybl_doc_attchmnts";
            pkNm = "doc_hdr_id";
            Global3.createAttachment(this.prmKeyID, nwDiag.attchmntNmTextBox.Text, "", tblNm, pkNm);
            long attchID = Global3.getAttchmntID(nwDiag.attchmntNmTextBox.Text, this.prmKeyID, tblNm, pkNm);
            if (Global3.mnFrm.cmCde1.copyAFile(attchID, this.fldrNm, nwDiag.fileNmTextBox.Text) == true)
            {
              Global3.updateAttachment(attchID, this.prmKeyID, nwDiag.attchmntNmTextBox.Text, attchID.ToString() + extnsn, tblNm, pkNm);
            }
          }
          else if (this.isPrchSng == false)
          {
            Global3.createAttachment(this.prmKeyID, nwDiag.attchmntNmTextBox.Text, "");
            long attchID = Global3.getAttchmntID(nwDiag.attchmntNmTextBox.Text, this.prmKeyID);
            if (Global3.mnFrm.cmCde1.copyAFile(attchID, Global3.mnFrm.cmCde1.getSalesImgsDrctry(), nwDiag.fileNmTextBox.Text) == true)
            {
              Global3.updateAttachment(attchID, this.prmKeyID, nwDiag.attchmntNmTextBox.Text, attchID.ToString() + extnsn);
            }
          }
          else
          {
            Global3.createP_Attachment(this.prmKeyID, nwDiag.attchmntNmTextBox.Text, "");
            long attchID = Global3.getP_AttchmntID(nwDiag.attchmntNmTextBox.Text, this.prmKeyID);
            if (Global3.mnFrm.cmCde1.copyAFile(attchID, Global3.mnFrm.cmCde1.getPrchsImgsDrctry(), nwDiag.fileNmTextBox.Text) == true)
            {
              Global3.updateP_Attachment(attchID, this.prmKeyID, nwDiag.attchmntNmTextBox.Text, attchID.ToString() + extnsn);
            }
          }
        }
      }
      this.gotoButton_Click(this.gotoButton, e);
    }

    private void editButton_Click(object sender, EventArgs e)
    {
      if (this.attchmntsListView.SelectedItems.Count <= 0)
      {
        Global3.mnFrm.cmCde1.showMsg("Please select an Item First!", 0);
        return;
      }
      string oldFile = "";
      if (this.attchCtgry == 3)
      {
        oldFile = this.fldrNm + @"\" +
          this.attchmntsListView.SelectedItems[0].SubItems[2].Text;
      }
      else if (this.isPrchSng == false)
      {
        oldFile = Global3.mnFrm.cmCde1.getSalesImgsDrctry() + @"\" +
                 this.attchmntsListView.SelectedItems[0].SubItems[2].Text;
      }
      else
      {
        oldFile = Global3.mnFrm.cmCde1.getPrchsImgsDrctry() + @"\" +
           this.attchmntsListView.SelectedItems[0].SubItems[2].Text;
      }
      string oldExtn = this.attchmntsListView.SelectedItems[0].SubItems[2].Text;
      addAttchmntDiag nwDiag = new addAttchmntDiag();
      nwDiag.attchCtgry = this.attchCtgry;
      nwDiag.attchmntIDTextBox.Text = this.attchmntsListView.SelectedItems[0].SubItems[3].Text;
      nwDiag.batchID = this.prmKeyID;
      nwDiag.isPrchSng = this.isPrchSng;
      nwDiag.attchmntNmTextBox.Text = this.attchmntsListView.SelectedItems[0].SubItems[1].Text;
      nwDiag.fileNmTextBox.Text = oldFile;
      DialogResult dgrs = nwDiag.ShowDialog();
      if (dgrs == DialogResult.OK)
      {
        if (Global3.mnFrm.cmCde1.myComputer.FileSystem.FileExists(
          nwDiag.fileNmTextBox.Text) == true)
        {
          //Global3.createAttachment(this.batchid, nwDiag.attchmntNmTextBox.Text, "");
          string tblNm = "";
          string pkNm = "";
          long attchID = long.Parse(nwDiag.attchmntIDTextBox.Text);
          string extnsn = Global3.mnFrm.cmCde1.myComputer.FileSystem.GetFileInfo(nwDiag.fileNmTextBox.Text).Extension;
          if (nwDiag.fileNmTextBox.Text != oldFile)
          {
            if (this.attchCtgry == 3)
            {
              tblNm = "accb.accb_pybl_doc_attchmnts";
              pkNm = "doc_hdr_id";
              if (Global3.mnFrm.cmCde1.copyAFile(attchID, this.fldrNm, nwDiag.fileNmTextBox.Text) == true)
              {
                Global3.updateAttachment(attchID, this.prmKeyID, nwDiag.attchmntNmTextBox.Text, attchID.ToString() + extnsn, tblNm, pkNm);
              }
            }
            else if (this.isPrchSng == false)
            {
              if (Global3.mnFrm.cmCde1.copyAFile(attchID, Global3.mnFrm.cmCde1.getSalesImgsDrctry(), nwDiag.fileNmTextBox.Text) == true)
              {
                Global3.updateAttachment(attchID, this.prmKeyID, nwDiag.attchmntNmTextBox.Text, attchID.ToString() + extnsn);
              }
            }
            else
            {
              if (Global3.mnFrm.cmCde1.copyAFile(attchID, Global3.mnFrm.cmCde1.getPrchsImgsDrctry(), nwDiag.fileNmTextBox.Text) == true)
              {
                Global3.updateAttachment(attchID, this.prmKeyID, nwDiag.attchmntNmTextBox.Text, attchID.ToString() + extnsn);
              }
            }
          }
          else
          {
            if (this.attchCtgry == 3)
            {
              tblNm = "accb.accb_pybl_doc_attchmnts";
              pkNm = "doc_hdr_id";
              Global3.mnFrm.cmCde1.upldImgsFTP(this.fldrTyp, this.fldrNm, oldExtn);
              Global3.updateAttachment(attchID, this.prmKeyID, nwDiag.attchmntNmTextBox.Text, oldExtn, tblNm, pkNm);
            }
            else
            {
              Global3.updateAttachment(attchID, this.prmKeyID, nwDiag.attchmntNmTextBox.Text, oldExtn);
            }
          }
        }
      }
      this.gotoButton_Click(this.gotoButton, e);
    }

    private void delButton_Click(object sender, EventArgs e)
    {
      if (this.attchmntsListView.SelectedItems.Count <= 0)
      {
        Global3.mnFrm.cmCde1.showMsg("Please select an Item First!", 0);
        return;
      }
      if (Global3.mnFrm.cmCde1.showMsg("NB: This action cannot be undone!\r\n" +
 "Are you sure you want to delete the selected Attachment?", 1) == DialogResult.No)
      {
        Global3.mnFrm.cmCde1.showMsg("Operation Cancelled!", 4);
        return;
      }
      //string oldFile = Global3.mnFrm.cmCde1.getSalesImgsDrctry() + @"\" +
      //  this.attchmntsListView.SelectedItems[0].SubItems[2].Text;
      string oldFile = "";
      string tblNm = "";
      if (this.attchCtgry == 3)
      {
        tblNm = "accb.accb_pybl_doc_attchmnts";
        oldFile = this.fldrNm + @"\" +
           this.attchmntsListView.SelectedItems[0].SubItems[2].Text;
      }
      else if (this.isPrchSng == false)
      {
        oldFile = Global3.mnFrm.cmCde1.getSalesImgsDrctry() + @"\" +
                 this.attchmntsListView.SelectedItems[0].SubItems[2].Text;
      }
      else
      {
        oldFile = Global3.mnFrm.cmCde1.getPrchsImgsDrctry() + @"\" +
           this.attchmntsListView.SelectedItems[0].SubItems[2].Text;
      }
      if (Global3.mnFrm.cmCde1.deleteAFile(oldFile) == true)
      {
        if (this.attchCtgry == 3)
        {
          Global3.deleteAttchmnt(long.Parse(this.attchmntsListView.SelectedItems[0].SubItems[3].Text),
    this.attchmntsListView.SelectedItems[0].SubItems[1].Text, tblNm);
        }
        else
        {
          Global3.deleteAttchmnt(long.Parse(this.attchmntsListView.SelectedItems[0].SubItems[3].Text),
            this.attchmntsListView.SelectedItems[0].SubItems[1].Text);
        }
      }
      this.gotoButton_Click(this.gotoButton, e);
    }

    private void openFileButton_Click(object sender, EventArgs e)
    {
      if (this.attchmntsListView.SelectedItems.Count > 0)
      {
        //      Global3.mnFrm.cmCde1.showMsg(Global3.mnFrm.cmCde1.getSalesImgsDrctry() +
        //@"\" + this.attchmntsListView.SelectedItems[0].SubItems[2].Text, 0);
        if (this.isPrchSng == false)
        {
          System.Diagnostics.Process.Start(Global3.mnFrm.cmCde1.getSalesImgsDrctry() +
           @"\" + this.attchmntsListView.SelectedItems[0].SubItems[2].Text);
        }
        else
        {
          System.Diagnostics.Process.Start(Global3.mnFrm.cmCde1.getPrchsImgsDrctry() +
   @"\" + this.attchmntsListView.SelectedItems[0].SubItems[2].Text);
        }
      }
    }

    private void attchmntsListView_DoubleClick(object sender, EventArgs e)
    {
      this.openFileButton.PerformClick();
    }
  }
}