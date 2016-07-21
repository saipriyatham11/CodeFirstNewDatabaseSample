using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Practice12
{
    public partial class New : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                string name = TextBox1.Text;
                string type = DropDownList1.SelectedValue;
                string filename = FileUpload1.FileName;
                FileUpload1.SaveAs(Server.MapPath("~/content/" + filename));
                Feedback.Text = "Submission Saved .";

            }

        }
    }
}