using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Odbc;

public partial class admin_UpgradePartII : System.Web.UI.Page
{
    protected void Page_PreInit(object sender, EventArgs e)
    {
        General g = Session["app"] as General;

        if (g.Auth.Status == false)
        {
            Session.Abandon();
            Response.Redirect("Default.aspx");
        }
        else
        {
            /*
             1 - users
             * 2 - menus
             * 3 - media
             * 4 - articles
             * 5 - tags
             * 6 - settings
             * 7 - group of media
             * 8 - languages
             * 9 - group of menus
             * 10 - subject
             * 11 - upgrade
             * Actions Allowed
             *  - SelectData
             *  - InsertData
             *  - UpdateData
             *  - DeleteData
             */
            //if (!g.Auth.CheckPermission(g.Auth.idUser, 11, "SelectData")) Response.Redirect("ControlPanel.aspx");
        }
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        General g = Session["app"] as General;

        string tmsg = "", SQL = "SELECT * "
            + "FROM VersionTab "
            + "ORDER BY DateInstallation";

        OdbcCommand cmd = g.DB.PrepareSQL(SQL);

        try
        {
            OdbcDataReader result = cmd.ExecuteReader();

            if (result.HasRows)
            {
                lblCVInstalled.Text = result["CurrentVersionDesc"].ToString();
                lblCVNInstalled.Text = result["CurrentVersionNum"].ToString();
                lblDateInstalled.Text = result["DateInstallation"].ToString();
                /*=========================================================*/
                lblUpgradeVersion.Text = "3.0.0 - 17/06/2009";
                lblUpgradeNumber.Text = "3.0";
                lblDateInstallationUpgrade.Text = DateTime.Now.ToString();
                if (float.Parse(lblCVNInstalled.Text) < float.Parse(lblUpgradeNumber.Text))
                {
                    tmsg = "Your current system is ready to be upgraded. Click in Commit to do that.";
                    btnUpgrade.Enabled = true;
                }
                else tmsg = "Your previous system is not ready to do Upgrade. Ask for John Mendes.";
            }
            else
            {
                tmsg = "Your previous system is not ready to do Upgrade. Ask for John Mendes.";
            }

            result.Close();
            result.Dispose();
        }
        catch (OdbcException o)
        {
            tmsg = g.DB.catchODBCException(o, g.ErrorLevel);
        }
        finally
        {
            cmd.Dispose();
            if (tmsg.Length == 0) msg.InnerHtml = "Ready to do Upgrade!";
            else msg.InnerHtml = tmsg;
        }

        ((ContentPlaceHolder)Master.FindControl("R3Enabled")).Visible = false;
        ((ContentPlaceHolder)Master.FindControl("R4Enabled")).Visible = false;
    }

    public void doUpgrade(object sender, EventArgs e)
    {
        General g = Session["app"] as General;

        string tmsg = "";
        Object[,] cmdSQL = new Object[1, 1];
        ArrayList sqlList = new ArrayList();

        OdbcCommand cmd = new OdbcCommand("", g.DB.connDB);

        cmdSQL[0, 0] = "versiontab";//OK
        cmdSQL[0, 1] = "Table";
        cmdSQL[1, 1] = "CREATE TABLE IF NOT EXISTS " + g.DB.databaseName + "." + cmdSQL[0, 1] + " ("
            + "idversion INT NOT NULL AUTO_INCREMENT, "
            + "currentversiondesc VARCHAR(45) NOT NULL, "
            + "currentversionnum INT NOT NULL, "
            + "dateinstallation DATETIME NOT NULL, "
            + "PRIMARY KEY (idversion))"
            + "ENGINE = InnoDB";
        sqlList.Add(cmdSQL);

        cmdSQL[0, 0] = "usertab";//OK
        cmdSQL[0, 1] = "Table";
        cmdSQL[1, 1] = "ALTER TABLE " + g.DB.databaseName + "." + cmdSQL[0, 1]
            + " MODIFY type ENUM('Admin', 'Standard') NOT NULL DEFAULT 'Admin' ";
        sqlList.Add(cmdSQL);

        cmdSQL[0, 0] = "usertab";//OK
        cmdSQL[0, 1] = "Table";
        cmdSQL[1, 1] = "UPDATE TABLE " + g.DB.databaseName + "." + cmdSQL[0, 1]
            + " SET type = 'Standard' "
            + "WHERE type='Regular'";
        sqlList.Add(cmdSQL);

        cmdSQL[0, 0] = "usertab";//OK
        cmdSQL[0, 1] = "Table";
        cmdSQL[1, 1] = "ALTER TABLE " + g.DB.databaseName + "." + cmdSQL[0, 1]
            + " CHANGE enabled BOOLEAN NOT NULL DEFAULT 1 AFTER date ";
        sqlList.Add(cmdSQL);

        cmdSQL[0, 0] = "languagetab";//OK
        cmdSQL[0, 1] = "Table";
        cmdSQL[1, 1] = "CREATE TABLE IF NOT EXISTS " + g.DB.databaseName + "." + cmdSQL[0, 1].ToString() + " ("
            + "idlanguage INT NOT NULL AUTO_INCREMENT, "
            + "title VARCHAR(45) NOT NULL, "
            + "abreviation VARCHAR(45) NOT NULL, "
            + "idmedia INT NOT NULL, "
            + "enabled BOOLEAN NOT NULL DEFAULT 1, "
            + "PRIMARY KEY (idlanguage)) "
            + "ENGINE = InnoDB";
        sqlList.Add(cmdSQL);

        cmdSQL[0, 0] = "languagetab";//OK
        cmdSQL[0, 1] = "References";
        cmdSQL[1, 1] = "ALTER TABLE " + g.DB.databaseName + "." + cmdSQL[0, 1].ToString() + " ADD "
            + "CONSTRAINT idmedia_languagetab_fk "
            + "FOREIGN KEY (idmedia) "
            + "REFERENCES " + g.DB.databaseName + ".mediatab (idmedia) "
            + "ON DELETE NO ACTION "
            + "ON UPDATE NO ACTION ";
        sqlList.Add(cmdSQL);

        cmdSQL[0, 0] = "languagetab";//OK
        cmdSQL[0, 1] = "Index";
        cmdSQL[1, 1] = "CREATE INDEX idmedia_languagetab_fk ON " + g.DB.databaseName + "." + cmdSQL[0, 1].ToString() + "(idmedia ASC)";
        sqlList.Add(cmdSQL);

        cmdSQL[0, 0] = "languagetab";//OK
        cmdSQL[0, 1] = "Datas";
        cmdSQL[1, 1] = "INSERT INTO " + g.DB.databaseName + "." + cmdSQL[0, 1] + " (title, abreviation, idmedia, enabled) VALUES ('English', 'en-gb', 1, 1)";
        sqlList.Add(cmdSQL);

        cmdSQL[0, 0] = "modulestab";//OK
        cmdSQL[0, 1] = "Table";
        cmdSQL[1, 1] = "CREATE TABLE IF NOT EXISTS " + g.DB.databaseName + "." + cmdSQL[0, 1] + " ("
            + "idmodule INT NOT NULL AUTO_INCREMENT, "
            + "title VARCHAR(45) NOT NULL, "
            + "description VARCHAR(255) NOT NULL, "
            + "enabled BOOLEAN NOT NULL DEFAULT 0, "
            + "PRIMARY KEY (idmodule)) "
            + "ENGINE = InnoDB";
        sqlList.Add(cmdSQL);

        cmdSQL[0, 0] = "modulestab";//OK
        cmdSQL[0, 1] = "Datas";
        cmdSQL[1, 1] = "INSERT INTO " + g.DB.databaseName + "." + cmdSQL[0, 1] + " (title, description) VALUES ('Users', 'Users Management')";
        sqlList.Add(cmdSQL);

        cmdSQL[0, 0] = "modulestab";//OK
        cmdSQL[0, 1] = "Datas";
        cmdSQL[1, 1] = "INSERT INTO " + g.DB.databaseName + "." + cmdSQL[0, 1].ToString() 
            + " (title, description) VALUES ('Menus', 'Menus Management')";
        sqlList.Add(cmdSQL);

        cmdSQL[0, 0] = "modulestab";//OK
        cmdSQL[0, 1] = "Datas";
        cmdSQL[1, 1] = "INSERT INTO " + g.DB.databaseName + "." + cmdSQL[0, 1].ToString() 
            + " (title, description) VALUES ('Media', 'Media Management')";
        sqlList.Add(cmdSQL);

        cmdSQL[0, 0] = "modulestab";//OK
        cmdSQL[0, 1] = "Datas";
        cmdSQL[1, 1] = "INSERT INTO " + g.DB.databaseName + "." + cmdSQL[0, 1].ToString() 
            + " (title, description) VALUES ('Articles', 'Articles Management')";
        sqlList.Add(cmdSQL);
        
        cmdSQL[0, 0] = "modulestab";//OK
        cmdSQL[0, 1] = "Datas";
        cmdSQL[1, 1] = "INSERT INTO " + g.DB.databaseName + "." + cmdSQL[0, 1].ToString() 
            + " (title, description) VALUES ('Tags', 'Tags Management')";
        sqlList.Add(cmdSQL);

        cmdSQL[0, 0] = "modulestab";//OK
        cmdSQL[0, 1] = "Datas";
        cmdSQL[1, 1] = "INSERT INTO " + g.DB.databaseName + "." + cmdSQL[0, 1].ToString() 
            + " (title, description) VALUES ('Settings', 'Settings Management')";
        sqlList.Add(cmdSQL);

        cmdSQL[0, 0] = "modulestab";//OK
        cmdSQL[0, 1] = "Datas";
        cmdSQL[1, 1] = "INSERT INTO " + g.DB.databaseName + "." + cmdSQL[0, 1].ToString() 
            + " (title, description) VALUES ('Group Media', 'Group of Media Management')";
        sqlList.Add(cmdSQL);
        
        cmdSQL[0, 0] = "modulestab";//OK
        cmdSQL[0, 1] = "Datas";
        cmdSQL[1, 1] = "INSERT INTO " + g.DB.databaseName + "." + cmdSQL[0, 1].ToString() 
            + " (title, description) VALUES ('Languages', 'Languages Management')";
        sqlList.Add(cmdSQL);
        
        cmdSQL[0, 0] = "modulestab";//OK
        cmdSQL[0, 1] = "Datas";
        cmdSQL[1, 1] = "INSERT INTO " + g.DB.databaseName + "." + cmdSQL[0, 1].ToString() 
            + " (title, description) VALUES ('Group Menu', 'Group of Menu Management')";
        sqlList.Add(cmdSQL);
        
        cmdSQL[0, 0] = "modulestab";//OK
        cmdSQL[0, 1] = "Datas";
        cmdSQL[1, 1] = "INSERT INTO " + g.DB.databaseName + "." + cmdSQL[0, 1].ToString() 
            + " (title, description) VALUES ('Subject', 'Subjects Management')";
        sqlList.Add(cmdSQL);
       
        cmdSQL[0, 0] = "modulestab";//OK
        cmdSQL[0, 1] = "Datas";
        cmdSQL[1, 1] = "INSERT INTO " + g.DB.databaseName + "." + cmdSQL[0, 1].ToString() 
            + " (title, description) VALUES ('Upgrade', 'Upgrade System')";
        sqlList.Add(cmdSQL);

        cmdSQL[0, 0] = "moduleusertab";//OK
        cmdSQL[0, 1] = "Table";
        cmdSQL[1, 1] = "CREATE TABLE IF NOT EXISTS " + g.DB.databaseName + "." + cmdSQL[0, 1].ToString() + " ("
            + "idmodule INT NOT NULL, "
            + "iduser INT NOT NULL, "
            + "selectdata BOOLEAN NOT NULL DEFAULT 0, "
            + "insertdata BOOLEAN NOT NULL DEFAULT 0, "
            + "updatedata BOOLEAN NOT NULL DEFAULT 0, "
            + "deletedata BOOLEAN NOT NULL DEFAULT 0, "
            + "PRIMARY KEY (idmodule, iduser)) "
            + "ENGINE = InnoDB";
        sqlList.Add(cmdSQL);

        cmdSQL[0, 0] = "moduleusertab";//OK
        cmdSQL[0, 1] = "References";
        cmdSQL[1, 1] = "ALTER TABLE " + g.DB.databaseName + "." + cmdSQL[0, 1].ToString() + " ADD "
            + "CONSTRAINT idmodule_moduleusertab_fk "
            + "FOREIGN KEY (idmodule) "
            + "REFERENCES " + g.DB.databaseName + ".modulestab (idmodule) "
            + "ON DELETE NO ACTION "
            + "ON UPDATE NO ACTION, ADD "
            + "CONSTRAINT iduser_moduleusertab_fk "
            + "FOREIGN KEY (iduser) "
            + "REFERENCES " + g.DB.databaseName + ".usertab (iduser) "
            + "ON DELETE NO ACTION "
            + "ON UPDATE NO ACTION ";
        sqlList.Add(cmdSQL);

        cmdSQL[0, 0] = "moduleusertab";//OK
        cmdSQL[0, 1] = "Index";
        cmdSQL[1, 1] = "CREATE INDEX idmodule_moduleusertab_fk ON " + g.DB.databaseName + "." 
            + cmdSQL[0, 1].ToString() + " (idmodule ASC)";
        sqlList.Add(cmdSQL);

        cmdSQL[0, 0] = "moduleusertab";//OK
        cmdSQL[0, 1] = "Index";
        cmdSQL[1, 1] = "CREATE INDEX iduser_moduleusertab_fk ON " + g.DB.databaseName + "." 
            + cmdSQL[0, 1].ToString() + " (iduser ASC)";
        sqlList.Add(cmdSQL);

        cmdSQL[0, 0] = "moduleusertab";//OK
        cmdSQL[0, 1] = "Data";
        cmdSQL[1, 1] = "INSERT INTO " + g.DB.databaseName + "." + cmdSQL[0, 1].ToString() 
            + " (idmodule, iduser, selectdata, updatedata, insertdata, deletedata) VALUES (1, 1, 1, 1, 1, 1)";
        sqlList.Add(cmdSQL);

        cmdSQL[0, 0] = "moduleusertab";//OK
        cmdSQL[0, 1] = "Data";
        cmdSQL[1, 1] = "INSERT INTO " + g.DB.databaseName + "." + cmdSQL[0, 1].ToString() 
            + " (idmodule, iduser, selectdata, updatedata, insertdata, deletedata) VALUES (2, 1, 1, 1, 1, 1)";
        sqlList.Add(cmdSQL);
        
        cmdSQL[0, 0] = "moduleusertab";//OK
        cmdSQL[0, 1] = "Data";
        cmdSQL[1, 1] = "INSERT INTO " + g.DB.databaseName + "." + cmdSQL[0, 1].ToString() 
            + " (idmodule, iduser, selectdata, updatedata, insertdata, deletedata) VALUES (3, 1, 1, 1, 1, 1)";
        sqlList.Add(cmdSQL);

        cmdSQL[0, 0] = "moduleusertab";//OK
        cmdSQL[0, 1] = "Data";
        cmdSQL[1, 1] = "INSERT INTO " + g.DB.databaseName + "." + cmdSQL[0, 1].ToString() 
            + " (idmodule, iduser, selectdata, updatedata, insertdata, deletedata) VALUES (4, 1, 1, 1, 1, 1)";
        sqlList.Add(cmdSQL);
        
        cmdSQL[0, 0] = "moduleusertab";//OK
        cmdSQL[0, 1] = "Data";
        cmdSQL[1, 1] = "INSERT INTO " + g.DB.databaseName + "." + cmdSQL[0, 1].ToString() 
            + " (idmodule, iduser, selectdata, updatedata, insertdata, deletedata) VALUES (5, 1, 1, 1, 1, 1)";
        sqlList.Add(cmdSQL);
        
        cmdSQL[0, 0] = "moduleusertab";//OK
        cmdSQL[0, 1] = "Data";
        cmdSQL[1, 1] = "INSERT INTO " + g.DB.databaseName + "." + cmdSQL[0, 1].ToString() 
            + " (idmodule, iduser, selectdata, updatedata, insertdata, deletedata) VALUES (6, 1, 1, 1, 1, 1)";
        sqlList.Add(cmdSQL);
        
        cmdSQL[0, 0] = "moduleusertab";//OK
        cmdSQL[0, 1] = "Data";
        cmdSQL[1, 1] = "INSERT INTO " + g.DB.databaseName + "." + cmdSQL[0, 1].ToString() 
            + " (idmodule, iduser, selectdata, updatedata, insertdata, deletedata) VALUES (7, 1, 1, 1, 1, 1)";
        sqlList.Add(cmdSQL);
        
        cmdSQL[0, 0] = "moduleusertab";//OK
        cmdSQL[0, 1] = "Data";
        cmdSQL[1, 1] = "INSERT INTO " + g.DB.databaseName + "." + cmdSQL[0, 1].ToString() 
            + " (idmodule, iduser, selectdata, updatedata, insertdata, deletedata) VALUES (8, 1, 1, 1, 1, 1)";
        sqlList.Add(cmdSQL);

        cmdSQL[0, 0] = "moduleusertab";//OK
        cmdSQL[0, 1] = "Data";
        cmdSQL[1, 1] = "INSERT INTO " + g.DB.databaseName + "." + cmdSQL[0, 1].ToString() 
            + " (idmodule, iduser, selectdata, updatedata, insertdata, deletedata) VALUES (9, 1, 1, 1, 1, 1)";
        sqlList.Add(cmdSQL);
        
        cmdSQL[0, 0] = "moduleusertab";//OK
        cmdSQL[0, 1] = "Data";
        cmdSQL[1, 1] = "INSERT INTO " + g.DB.databaseName + "." + cmdSQL[0, 1].ToString() 
            + " (idmodule, iduser, selectdata, updatedata, insertdata, deletedata) VALUES (10, 1, 1, 1, 1, 1)";
        sqlList.Add(cmdSQL);
       
        cmdSQL[0, 0] = "subjecttab";//OK
        cmdSQL[0, 1] = "Table";
        cmdSQL[1, 1] = "CREATE TABLE IF NOT EXISTS " + g.DB.databaseName + "." + cmdSQL[0, 1].ToString() + " ("
            + "idsubject INT NOT NULL AUTO_INCREMENT, "
            + "title VARCHAR(45) NOT NULL, "
            + "description VARCHAR(255) NOT NULL, "
            + "PRIMARY KEY (idsubject)) "
            + "ENGINE = InnoDB";
        sqlList.Add(cmdSQL);

        cmdSQL[0, 0] = "menugrouptab";//OK
        cmdSQL[0, 1] = "Table";
        cmdSQL[1, 1] = "CREATE  TABLE IF NOT EXISTS " + g.DB.databaseName + "." + cmdSQL[0, 1].ToString() + " ("
            + "idgroupmenu INT NOT NULL AUTO_INCREMENT, "
            + "title VARCHAR(100) NOT NULL, "
            + "idlanguage INT NOT NULL, "
            + "enabled BOOLEAN NOT NULL DEFAULT 1, "
            + "PRIMARY KEY (idgroupmenu)) "
            + "ENGINE = InnoDB";
        sqlList.Add(cmdSQL);

        cmdSQL[0, 0] = "menugrouptab";//OK
        cmdSQL[0, 1] = "References";
        cmdSQL[1, 1] = "ALTER TABLE " + g.DB.databaseName + "." + cmdSQL[0, 1].ToString() + " ADD "
            + "CONSTRAINT idlanguage_menutab_fk "
            + "FOREIGN KEY (idlanguage) "
            + "REFERENCES " + g.DB.databaseName + ".languagetab (idlanguage) "
            + "ON DELETE NO ACTION "
            + "ON UPDATE NO ACTION ";
        sqlList.Add(cmdSQL);

        cmdSQL[0, 0] = "menugrouptab";//OK
        cmdSQL[0, 1] = "Index";
        cmdSQL[1, 1] = "CREATE INDEX idlanguage_menutab_fk ON " + g.DB.databaseName + "." + cmdSQL[0, 1].ToString() 
            + " (idlanguage ASC)";
        sqlList.Add(cmdSQL);

        cmdSQL[0, 0] = "menugrouptab";//OK
        cmdSQL[0, 1] = "Datas";
        cmdSQL[1, 1] = "INSERT INTO " + g.DB.databaseName + "." + cmdSQL[0, 1].ToString() 
            + " (title, idlanguage, enabled) VALUES ('English Menus', 1, 1)";
        sqlList.Add(cmdSQL);

        /*
         need 
         */

        cmdSQL[0, 0] = "menutab";//OK
        cmdSQL[0, 1] = "Table";
        cmdSQL[1, 1] = "ALTER TABLE " + g.DB.databaseName + "." + cmdSQL[0, 1]
            + " ADD COLUMN idgroupmenu INT NOT NULL AFTER idmenu ";
        sqlList.Add(cmdSQL);

        cmdSQL[0, 0] = "menutab";//OK
        cmdSQL[0, 1] = "Table";
        cmdSQL[1, 1] = "ALTER TABLE " + g.DB.databaseName + "." + cmdSQL[0, 1]
            + " CHANGE enabled TINYINT (1) NOT NULL DEFAULT 1 AFTER visible ";
        sqlList.Add(cmdSQL);

        cmdSQL[0, 0] = "menutab";//OK
        cmdSQL[0, 1] = "Table";
        cmdSQL[1, 1] = "ALTER TABLE " + g.DB.databaseName + "." + cmdSQL[0, 1]
            + " DROP FOREIGN KEY idmenu_menutab_fk ";
        sqlList.Add(cmdSQL);

        cmdSQL[0, 0] = "menutab";//OK
        cmdSQL[0, 1] = "Table";
        cmdSQL[1, 1] = "ALTER TABLE " + g.DB.databaseName + "." + cmdSQL[0, 1]
            + " DROP INDEX idmenu_menutab_fk ";
        sqlList.Add(cmdSQL);

        cmdSQL[0, 0] = "menutab";//OK
        cmdSQL[0, 1] = "Table";
        cmdSQL[1, 1] = "ALTER TABLE " + g.DB.databaseName + "." + cmdSQL[0, 1]
            + " CHANGE sidmenu idmenuparent INT NULL AFTER position ";
        sqlList.Add(cmdSQL);

        cmdSQL[0, 0] = "menutab";//OK
        cmdSQL[0, 1] = "Table";
        cmdSQL[1, 1] = "ALTER TABLE " + g.DB.databaseName + "." + cmdSQL[0, 1].ToString() + " ADD "
            + "CONSTRAINT idmenuparent_menutab_fk "
            + "FOREIGN KEY (idmenuparent) "
            + "REFERENCES " + g.DB.databaseName + ".menutab (idmenu) "
            + "ON DELETE NO ACTION "
            + "ON UPDATE NO ACTION, ADD "
            +"CONSTRAINT idgroupmenu_menutab_fk "
            + "FOREIGN KEY (idgroupmenu) "
            + "REFERENCES " + g.DB.databaseName + ".menutab (idmenu) "
            + "ON DELETE NO ACTION "
            + "ON UPDATE NO ACTION, ADD ";

        sqlList.Add(cmdSQL);

        cmdSQL[0, 0] = "menutab";//OK
        cmdSQL[0, 1] = "Index";
        cmdSQL[1, 1] = "CREATE INDEX idmenuparent_menutab_fk ON " + g.DB.databaseName + "."
            + cmdSQL[0, 1].ToString() + " (idmenuparent ASC)";
        sqlList.Add(cmdSQL);


        /*
         * 
         * 

        SQL[2, 1] = "articletab";
        SQL[2, 2] = "Table";
        SQL[2, 0] = "CREATE  TABLE IF NOT EXISTS " + txtDBName.Text + "." + SQL[2, 1].ToString() + " (";
        SQL[2, 0] += "idarticle INT NOT NULL AUTO_INCREMENT, ";
        SQL[2, 0] += "title VARCHAR(100) NOT NULL, ";
        SQL[2, 0] += "description LONGTEXT NOT NULL, ";
        SQL[2, 0] += "idmenu INT NOT NULL, ";
        SQL[2, 0] += "iduser INT NOT NULL, ";
        SQL[2, 0] += "idmainmedia INT NULL, ";
        SQL[2, 0] += "type ENUM('Page', 'Media', 'News') NOT NULL DEFAULT 'Page', ";
        SQL[2, 0] += "published DATETIME NOT NULL, ";
        SQL[2, 0] += "expires DATETIME NULL, ";
        SQL[2, 0] += "lastupdate DATETIME NULL, ";
        SQL[2, 0] += "enabled BOOLEAN NOT NULL DEFAULT 0, ";
        SQL[2, 0] += "PRIMARY KEY (idarticle)) ";
        SQL[2, 0] += "ENGINE = InnoDB";

        SQL[5, 1] = "mediatab";
        SQL[5, 2] = "Table";
        SQL[5, 0] = "CREATE  TABLE IF NOT EXISTS " + txtDBName.Text + "." + SQL[5, 1].ToString() + " (";
        SQL[5, 0] += "idmedia INT NOT NULL AUTO_INCREMENT, ";
        SQL[5, 0] += "title VARCHAR(255) NOT NULL, ";
        SQL[5, 0] += "originalname VARCHAR(255) NOT NULL, ";
        SQL[5, 0] += "filename VARCHAR(45) NOT NULL, ";
        SQL[5, 0] += "enabled BOOLEAN NOT NULL DEFAULT 0, ";
        SQL[5, 0] += "PRIMARY KEY (idmedia)) ";
        SQL[5, 0] += "ENGINE = InnoDB";

        SQL[6, 1] = "configtab";
        SQL[6, 2] = "Table";
        SQL[6, 0] = "CREATE  TABLE IF NOT EXISTS " + txtDBName.Text + "." + SQL[6, 1].ToString() + " (";
        SQL[6, 0] += "idconfig INT NOT NULL AUTO_INCREMENT, ";
        SQL[6, 0] += "title VARCHAR(255) NOT NULL, ";
        SQL[6, 0] += "idlogo INT NULL, ";
        SQL[6, 0] += "numbernews INT NOT NULL DEFAULT 5, ";
        SQL[6, 0] += "idarticlea INT NULL, ";
        SQL[6, 0] += "idarticleheader INT NULL, ";
        SQL[6, 0] += "idarticlefooter INT NULL, ";
        SQL[6, 0] += "folderinstalled VARCHAR(255) NULL, ";
        SQL[6, 0] += "menustyle ENUM('Original', 'Tree', 'Top Bar', 'Side Bar') NOT NULL DEFAULT 'Original', ";
        SQL[6, 0] += "description LONGTEXT NOT NULL, ";
        SQL[6, 0] += "keywords LONGTEXT NOT NULL, ";
        SQL[6, 0] += "enabled INT NOT NULL DEFAULT 1, ";
        SQL[6, 0] += "PRIMARY KEY (idconfig)) ";
        SQL[6, 0] += "ENGINE = InnoDB";

        SQL[9, 1] = "articletab";
        SQL[9, 2] = "References";
        SQL[9, 0] = "ALTER TABLE " + txtDBName.Text + "." + SQL[9, 1].ToString() + " ADD ";
        SQL[9, 0] += "CONSTRAINT idmenu_articletab_fk ";
        SQL[9, 0] += "FOREIGN KEY (idmenu) ";
        SQL[9, 0] += "REFERENCES " + txtDBName.Text + ".menutab (idmenu) ";
        SQL[9, 0] += "ON DELETE NO ACTION ";
        SQL[9, 0] += "ON UPDATE NO ACTION, ADD ";
        SQL[9, 0] += "CONSTRAINT iduser_articletab_fk ";
        SQL[9, 0] += "FOREIGN KEY (iduser) ";
        SQL[9, 0] += "REFERENCES " + txtDBName.Text + ".usertab (iduser) ";
        SQL[9, 0] += "ON DELETE NO ACTION ";
        SQL[9, 0] += "ON UPDATE NO ACTION, ADD ";
        SQL[9, 0] += "CONSTRAINT idmedia_articletab_fk ";
        SQL[9, 0] += "FOREIGN KEY (idmainmedia) ";
        SQL[9, 0] += "REFERENCES " + txtDBName.Text + ".mediatab (idmedia) ";
        SQL[9, 0] += "ON DELETE NO ACTION ";
        SQL[9, 0] += "ON UPDATE NO ACTION ";

        SQL[11, 1] = "configtab";
        SQL[11, 2] = "References";
        SQL[11, 0] = "ALTER TABLE " + txtDBName.Text + "." + SQL[11, 1].ToString() + " ADD ";
        SQL[11, 0] += "CONSTRAINT idarticlea_configtab_fk ";
        SQL[11, 0] += "FOREIGN KEY (idarticlea) ";
        SQL[11, 0] += "  REFERENCES " + txtDBName.Text + ".articletab (idarticle) ";
        SQL[11, 0] += "  ON DELETE NO ACTION ";
        SQL[11, 0] += "  ON UPDATE NO ACTION, ADD ";
        SQL[11, 0] += "CONSTRAINT idmedia_configtab_fk ";
        SQL[11, 0] += "  FOREIGN KEY (idlogo) ";
        SQL[11, 0] += "  REFERENCES " + txtDBName.Text + ".mediatab (idmedia) ";
        SQL[11, 0] += "  ON DELETE NO ACTION ";
        SQL[11, 0] += "  ON UPDATE NO ACTION, ADD ";
        SQL[11, 0] += "CONSTRAINT idarticlefooter_configtab_fk ";
        SQL[11, 0] += "  FOREIGN KEY (idarticlefooter) ";
        SQL[11, 0] += "  REFERENCES " + txtDBName.Text + ".articletab (idarticle) ";
        SQL[11, 0] += "  ON DELETE NO ACTION ";
        SQL[11, 0] += "  ON UPDATE NO ACTION, ADD ";
        SQL[11, 0] += "CONSTRAINT idarticleheader_configtab_fk ";
        SQL[11, 0] += "  FOREIGN KEY (idarticleheader) ";
        SQL[11, 0] += "  REFERENCES " + txtDBName.Text + ".articletab (idarticle) ";
        SQL[11, 0] += "  ON DELETE NO ACTION ";
        SQL[11, 0] += "  ON UPDATE NO ACTION ";


        SQL[14, 1] = "articletab";
        SQL[14, 2] = "Index";
        SQL[14, 0] = "CREATE INDEX idmenu_articletab_fk ON " + txtDBName.Text + "." + SQL[14, 1].ToString() + " (idmenu ASC)";

        SQL[15, 1] = "articletab";
        SQL[15, 2] = "Index";
        SQL[15, 0] = "CREATE INDEX idmedia_articletab_fk ON " + txtDBName.Text + "." + SQL[15, 1].ToString() + " (idmainmedia ASC)";

        SQL[16, 1] = "articletagtab";
        SQL[16, 2] = "Index";
        SQL[16, 0] = "CREATE INDEX idarticle_articletagtab_fk ON " + txtDBName.Text + "." + SQL[16, 1].ToString() + " (idarticle ASC)";

        SQL[17, 1] = "articletagtab";
        SQL[17, 2] = "Index";
        SQL[17, 0] = "CREATE INDEX idtag_articletagtab_fk ON " + txtDBName.Text + "." + SQL[17, 1].ToString() + " (idtag ASC)";

        SQL[18, 1] = "configtab";
        SQL[18, 2] = "Index";
        SQL[18, 0] = "CREATE INDEX idarticlea_configtab_fk ON " + txtDBName.Text + "." + SQL[18, 1].ToString() + " (idarticlea ASC)";

        SQL[19, 1] = "configtab";
        SQL[19, 2] = "Index";
        SQL[19, 0] = "CREATE INDEX idarticleheader_configtab_fk ON " + txtDBName.Text + "." + SQL[19, 1].ToString() + " (idarticleheader ASC)";

        SQL[20, 1] = "configtab";
        SQL[20, 2] = "Index";
        SQL[20, 0] = "CREATE INDEX idmedia_configtab_fk ON " + txtDBName.Text + "." + SQL[20, 1].ToString() + " (idlogo ASC)";

        SQL[21, 1] = "configtab";
        SQL[21, 2] = "Index";
        SQL[21, 0] = "CREATE INDEX idarticlefooter_configtab_fk ON " + txtDBName.Text + "." + SQL[21, 1].ToString() + " (idarticlefooter ASC)";

        SQL[25, 1] = "mediatab";
        SQL[25, 2] = "Datas";
        SQL[25, 0] = "INSERT INTO " + txtDBName.Text + "." + SQL[25, 1].ToString() + " (title, originalname, filename, enabled) VALUES ('Logo John Mendes', 'JohnMendesLogo.gif', '1.gif', 1)";

        SQL[26, 1] = "configtab";
        SQL[26, 2] = "Datas";
        SQL[26, 0] = "INSERT INTO " + txtDBName.Text + "." + SQL[26, 1].ToString() + " (idconfig, title, idlogo, numbernews, idarticlea, idarticleheader, idarticlefooter, folderinstalled, menustyle, description, keywords, enabled) VALUES (0, 'John Mendes CMS Web Site', 1, 5, NULL, NULL, NULL, '', 'Original', '', '', 0)";

        SQL[27, 1] = "articletab";
        SQL[27, 2] = "Index";
        SQL[27, 0] = "CREATE INDEX iduser_articletab_fk ON " + txtDBName.Text + "." + SQL[27, 1].ToString() + " (iduser ASC)";

        */

        msg.InnerHtml = "Done!" + sqlList[0].ToString();
    }
}
