using System;
using System.Collections.Generic;
using System.Linq;
using BCN.HydrantWiki.Library.Managers;
using BCN.Library.Objects;
using BCN.HydrantWiki.Library.Objects;
using System.Web.UI.HtmlControls;
using System.Collections;
using BCN.Web.Library.Helpers;
using Telerik.Web.UI;
using HydrantWiki.Library.Objects;

namespace HydrantWiki
{
    public partial class MyTags : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            User user = (User) Session["User"];

            if (user == null)
            {
                Response.Redirect("/default.aspx");
            }

            if (!IsPostBack)
            {
                HtmlGenericControl mybody = (this.Master as HydrantWiki.App.App).Body;
                mybody.Attributes.Add("onload", "setupMap()");

                Session["MyTagsStartKey"] = null;
                LoadNextPage();
            }
        }

        private void LoadNextPage()
        {
            User user = (User)Session["User"];
            
            if (user != null)
            {
                string startKey = (string)Session["MyTagsStartKey"];
                ServerDataManager sdm = new ServerDataManager();
                List<TagRow> tags = sdm.GetTagRows(user.UserName, true, 10, ref startKey);
                Session["MyTags"] = tags;
                Session["MyTagsStartKey"] = startKey;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gridTags_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            User user = (User)Session["User"];

            if (user != null)
            {
                List<TagRow> tags = (List<TagRow>) Session["MyTags"];
                gridTags.DataSource = tags;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            User user = (User)Session["User"];

            if (user != null)
            {
                ServerDataManager sdm = new ServerDataManager();

                GridItemCollection items = gridTags.SelectedItems;

                if (items.Count > 0)
                {
                    GridDataItem gdi = (GridDataItem)items[0];

                    Hashtable values = new Hashtable();
                    gdi.ExtractValues(values);

                    Guid tagGuid = Guid.Parse(values["TagGuid"] as string);

                    Tag tag = sdm.GetTag(user.UserName, tagGuid);

                    if (tag != null)
                    {
                        tag.Active = false;

                        sdm.PersistInactiveTag(tag);
                        sdm.DeleteTag(tag);

                        List<TagRow> tags = (List<TagRow>)Session["MyTags"];
                        TagRow deleted = null;

                        foreach (TagRow tagRow in tags)
                        {
                            if (tagRow.TagGuid.Equals(tag.Guid))
                            {
                                deleted = tagRow;
                                break;
                            }
                        }

                        tags.Remove(deleted);
                        Session["MyTags"] = tags;

                        gridTags.Rebind();
                    }
                    else
                    {
                        LoggingHelper.Warning(sdm.SQSClient, user.UserName, "Tag not found.");
                    }

                }
                else
                {
                    //No row selected
                }
            }
            else
            {
                //No session
            }
        }

        protected void btnNextPage_Click(object sender, EventArgs e)
        {
            LoadNextPage();

            gridTags.Rebind();
        }

        protected void btnFirstPage_Click(object sender, EventArgs e)
        {
            Session["MyTagsStartKey"] = null;
            LoadNextPage();

            gridTags.Rebind();
        }
    }
}