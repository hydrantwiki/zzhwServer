using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using BCN.Library.Objects;
using BCN.Library.Enums;
using BCN.HydrantWiki.Library.Managers;
using BCN.Web.Library.Helpers;
using HydrantWiki.Library.Objects;
using BCN.Library.Helpers;
using BCN.HydrantWiki.Library.Objects;
using BCN.HydrantWiki.Library.Helpers;
using System.Web.UI.HtmlControls;

namespace HydrantWiki
{
    public partial class ReviewTags : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            User user = (User)Session["User"];

            if (user == null)
            {
                Response.Redirect("/default.aspx");
            }

            if (user.UserType == UserTypes.Administrator
                || user.UserType == UserTypes.SuperUser)
            {

            }
            else
            {
                Response.Redirect("/Home.aspx");
            }
            
            if (!IsPostBack)
            {
                HtmlGenericControl mybody = (this.Master as HydrantWiki.App.App).Body;
                mybody.Attributes.Add("onload", "setupMap()");

                btnAccept.Enabled = false;
                btnReject.Enabled = false;
            }
        }

        private void GetNextTag()
        {
            Session["CurrentReviewTag"] = null;
            Session["CurrentReceiptHandle"] = null;
            btnAccept.Enabled = false;
            btnReject.Enabled = false;

            ServerDataManager sdm = new ServerDataManager();
            string queueURL = Config.getSettingValue("SQSPendingTagQueue");
            bool itemFound = false;

            string receiptHandle;
            BCNSerializedObject obj = SQSHelper.ReceiveMessage(sdm.SQSClient,
                queueURL,
                out receiptHandle);

            if (obj != null
                && receiptHandle != null)
            {
                Tag tag = BCNSerializedObject.GetBCNSerializable<Tag>(obj);

                if (tag != null)
                {
                    Tag tag2 = sdm.GetTag(tag.UserName, tag.Guid);

                    if (tag2 == null)
                    {
                        GetNextTag();
                        return;
                    }

                    Session["CurrentReviewTag"] = tag;
                    Session["CurrentReceiptHandle"] = receiptHandle;

                    lblUserID.Text = string.Format("User ID: {0}", tag.UserName);
                    lblTagDateTime.Text = string.Format("Tag Date Time: {0}", tag.DeviceDateTime.ToString("u"));
                    imgTagImage.ImageUrl = tag.GetUrl(true);

                    if (tag.Position != null)
                    {
                        hidLatitude.Value = Convert.ToString(tag.Position.Y);
                        hidLongitude.Value = Convert.ToString(tag.Position.X);

                        List<NearbyHydrant> nearby = sdm.GetNearbyHydrants(tag.Position);
                        gridNearby.DataSource = nearby;
                        gridNearby.DataBind();

                        if (nearby.Count > 0)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("lat,lon,title,marker-color\n");
                            int i = 1;
                            foreach (NearbyHydrant nbh in nearby)
                            {
                                sb.AppendFormat("{0},{1},{2},#180392\n", nbh.Position.Y, nbh.Position.X, i);
                                i++;
                            }
                            if (sb.Length > 0)
                            {
                                sb.Remove(sb.Length - 1, 1);
                            }

                            hidNearbyPointsCSV.Value = sb.ToString();
                        }
                        else
                        {
                            hidNearbyPointsCSV.Value = null;
                        }

                        string scriptstring = "refreshMap();";
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "refreshMap", scriptstring, true);
                    }

                    btnAccept.Enabled = true;
                    btnReject.Enabled = true;
                }
            }
            else
            {
                lblUserID.Text = "User ID: No Tags Found";
                lblTagDateTime.Text = "Tag Date Time: No Tags Found";
                imgTagImage.ImageUrl = null;
                gridNearby.DataSource = null;
                gridNearby.Rebind();
            }
        }

        protected void btnGetTag_Click(object sender, EventArgs e)
        {
            GetNextTag();
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            Tag tag = (Tag)Session["CurrentReviewTag"];
            User user = (User)Session["User"];

            if (tag != null
                && user != null)
            {
                ServerDataManager sdm = new ServerDataManager();

                RejectedTag rt = new RejectedTag();
                rt.UserName = tag.UserName;
                rt.DeviceDateTime = tag.DeviceDateTime;
                rt.RejectedByUserName = user.UserName;
                rt.Reason = null;
                rt.RejectedDateTime = DateTime.UtcNow;
                rt.TagGuid = tag.Guid;
                sdm.Persist(rt);

                sdm.DeletePendingTag(tag);

                DeleteMessage(sdm);
            }

            GetNextTag();
        }

        private void DeleteMessage(ServerDataManager sdm)
        {
            string currentHandle = (string)Session["CurrentReceiptHandle"];
            string queueURL = Config.getSettingValue("SQSPendingTagQueue");

            SQSHelper.DeleteMessage(sdm.SQSClient, queueURL, currentHandle); 
        }

        protected void btnAccept_Click(object sender, EventArgs e)
        {
            Tag tag = (Tag)Session["CurrentReviewTag"];
            User user = (User)Session["User"];

            if (tag != null
                && user != null)
            {
                ServerDataManager sdm = new ServerDataManager();

                if (gridNearby.SelectedItems != null
                    && gridNearby.SelectedItems.Count > 0)
                {
                    Guid? hydrantGuid = null;

                    if (gridNearby.MasterTableView.DataKeyValues.Count > 0)
                    {
                        Hashtable values = gridNearby.MasterTableView.DataKeyValues[0];
                        if (values["HydrantGuid"] != null)
                        {
                            hydrantGuid = (Guid)values["HydrantGuid"];
                        }
                    }

                    if (hydrantGuid != null)
                    {
                        Hydrant h = sdm.GetHydrant(hydrantGuid.Value);

                        if (h != null)
                        {
                            if (tag.ImageGuid != null)
                            {
                                sdm.AssignImageToHydrant(h.Guid, tag.ImageGuid.Value);
                            }

                            sdm.AssignUserToHydrant(h.Guid, tag.UserName);

                            h.Position = PositionHelper.Average(h.Position, tag.Position);

                            sdm.Persist(h);

                            if (tag.ExternalSource == "OSM")
                            {
                                OSMHydrant osm = new OSMHydrant();
                                osm.OSMID = Convert.ToInt64(tag.ExternalIdentifier);
                                osm.HydrantGuid = h.Guid;
                                sdm.Persist(osm);
                            }

                            //This tag has been handled.
                            sdm.DeletePendingTag(tag);
                            DeleteMessage(sdm);
                        }
                    }
                }
                else
                {
                    //Not an existing hydrant
                    Hydrant h = sdm.TagToHydrant(tag, user.UserName);
                   
                    if (tag.ExternalSource == "OSM")
                    {
                        OSMHydrant osm = new OSMHydrant();
                        osm.OSMID = Convert.ToInt64(tag.ExternalIdentifier);
                        osm.HydrantGuid = h.Guid;
                        sdm.Persist(osm);
                    }

                    sdm.DeletePendingTag(tag);
                    DeleteMessage(sdm);
                }
            }

            GetNextTag();
        }
    }
}