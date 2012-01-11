using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

using De.AHoerstemeier.Tambon;

public partial class AmphoeDataDisplay : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            TambonHelper.BaseXMLDirectory = Request.PhysicalApplicationPath + "\\App_Data";
            FillChangwatCombobox();
        }
    }

    protected void FillChangwatCombobox()
    {
        if (TambonHelper.ProvinceGeocodes == null)
        {
            TambonHelper.LoadGeocodeList();
        }

        cbx_changwat.Items.Clear();
        foreach (PopulationDataEntry lEntry in TambonHelper.ProvinceGeocodes)
        {
            cbx_changwat.Items.Add(new ListItem(lEntry.English,lEntry.Geocode.ToString()));
        }
    }

    protected void cbx_amphoe_SelectedIndexChanged(object sender, EventArgs e)
    {
        
    }
    protected void cbx_changwat_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList lSender = sender as DropDownList;
        Int32 lGeocode = Convert.ToInt32(lSender.SelectedValue);
        PopulationData lGeocodes = null;
        cbx_amphoe.Items.Clear();
            lGeocodes = TambonHelper.GetGeocodeList(lGeocode);
            foreach (PopulationDataEntry lEntry in lGeocodes.Data.SubEntities)
            {
                if (!lEntry.Obsolete)
                {
                    cbx_amphoe.Items.Add(new ListItem(lEntry.English, lEntry.Geocode.ToString()));
                }
            }
    }
}
