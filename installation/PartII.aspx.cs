using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Text;
using System.Collections;

public partial class installation_PartII : System.Web.UI.Page
{
    public OdbcConnection connDB;

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public void createDB(object sender, EventArgs e)
    {
        //string connectionString = "Driver={MySql ODBC 5.1 Driver}; Server=" + txtHost.Text + "; uid="
        //+ txtUserName.Text +"; pwd="+ txtPassword.Text +"; database="+ txtDBName.Text +"; option=3; port="
        //+ txtPort.Text +";";
        string connectionString = "Driver={MySql ODBC "+txtMySQLDriverVersion.Text+" Driver}; Server=" 
            + txtHost.Text + "; uid=" + txtUserName.Text + "; pwd=" + txtPassword.Text + "; option=3; port=" 
            + txtPort.Text + ";";
        this.connDB = new OdbcConnection(connectionString);
        this.connDB.Open();

        string tmsg = "", SQL = "";


        SQL = "CREATE DATABASE IF NOT EXISTS " + txtDBName.Text + " DEFAULT CHARACTER SET utf8 COLLATE "
         + "utf8_general_ci";

        OdbcCommand cmd = new OdbcCommand(SQL, connDB);

        try
        {
            cmd.ExecuteNonQuery();
        }
        catch (OdbcException o)
        {
            tmsg = "We are sorry, we are experiencing technical problems ...";
            tmsg += "<hr/>";
            tmsg += "<br/>Message: " + o.Message;
            tmsg += "<br/>Source: " + o.Source;
            tmsg += "<br/>Stack Trace: " + o.StackTrace;
            tmsg += "<br/>Target Site: " + o.TargetSite.Name;
            tmsg += "<br/>Others: " + o.InnerException;
            tmsg += "<hr/>";
            tmsg += "<br/>Try again! If the problem persist, contact techinical suport";
        }
        finally
        {
            cmd.Dispose();
            if (tmsg.Length == 0) {
                msg.InnerHtml = "Database: " + txtDBName.Text + " created successfully!<br/>";
                if (createTables(sender, e))
                {
                    btnCreateDB.Enabled = false;                    
                    if (createConfigFile(sender, e))
                    {
                        btnNext.Enabled = true;
                    }
                }
            }else msg.InnerHtml = tmsg;
        }
        Session.Abandon();
    }

    protected bool createConfigFile(object sender, EventArgs e) 
    {
        string tmsg = "", pathConfigFile = Server.MapPath("~") + "\\config.txt";
        bool result = false;        
        
        try
        {
            TextWriter ConfigFile = new StreamWriter(pathConfigFile);
            ConfigFile.WriteLine(txtHost.Text);
            ConfigFile.WriteLine(txtPort.Text);
            ConfigFile.WriteLine(txtUserName.Text);
            ConfigFile.WriteLine(txtPassword.Text);
            ConfigFile.WriteLine(txtDBName.Text);
            ConfigFile.WriteLine(txtMySQLDriverVersion.Text);
            ConfigFile.Close();
        }
        catch (Exception ex)
        {
            tmsg = "We cannot write a file at your folder. Ckeck if you have permission to write at your web site.";
            tmsg += "<hr/>";
            tmsg += "<br/>Message: " + ex.Message;
            tmsg += "<br/>Source: " + ex.Source;
            tmsg += "<br/>Stack Trace: " + ex.StackTrace;
            tmsg += "<br/>Target Site: " + ex.TargetSite.Name;
            tmsg += "<br/>Others: " + ex.InnerException;
            tmsg += "<hr/>";
            tmsg += "<br/>Try again! If the problem persist, contact techinical suport";
        }
        finally
        {
            if (tmsg.Length == 0)
            {
                msg.InnerHtml += "<br/>Config.txt file created successfully!<br/>";
                result = true;
            }
            else
            {
                msg.InnerHtml += tmsg;
                result = false;
            }
        }

        return result;
    }

    private string[,] ReturnSQLINArray(string TableName, string type, string sql)
    {
        string[,] SQL = new string[1, 3];

        SQL[0, 0] = sql; 
        SQL[0, 1] = TableName;
        SQL[0, 2] = type;

        return SQL;
    }

    protected bool createTables(object sender, EventArgs e)
    {
        string tmsg = "";
        
        ArrayList SQLList = new ArrayList();
        bool result = false;

        OdbcCommand cmd = new OdbcCommand("", this.connDB);

        SQLList.Add(ReturnSQLINArray("friendlyurltab", "Table", 
            "CREATE TABLE IF NOT EXISTS " + txtDBName.Text + ".friendlyurltab ("
            + "idfriendlyurl INT NOT NULL AUTO_INCREMENT, "
            + "idarticle INT NOT NULL, "
            + "friendlyurl VARCHAR(500) NOT NULL, "
            + "PRIMARY KEY (idfriendlyurl))"
            + "ENGINE = InnoDB"));
        
        SQLList.Add(ReturnSQLINArray("versiontab", "Table",
            "CREATE TABLE IF NOT EXISTS " + txtDBName.Text + ".versiontab ("
            + "idversion INT NOT NULL AUTO_INCREMENT, "
            + "currentversiondesc VARCHAR(45) NOT NULL, "
            + "currentversionnum INT NOT NULL, "
            + "dateinstallation DATETIME NOT NULL, "
            + "PRIMARY KEY (idversion))"
            + "ENGINE = InnoDB"));

        SQLList.Add(ReturnSQLINArray("usertab", "Table",
            "CREATE  TABLE IF NOT EXISTS " + txtDBName.Text + ".usertab ("
            + "iduser INT NOT NULL AUTO_INCREMENT, "
            + "name VARCHAR(25) NOT NULL, "
            + "surname VARCHAR(25) NOT NULL, "
            + "username VARCHAR(150) NOT NULL, "
            + "password VARCHAR(32) NOT NULL, "
            + "email VARCHAR(255) NOT NULL, "
            + "type ENUM('Admin', 'Standard') NOT NULL DEFAULT 'Admin', "
            + "date DATETIME NOT NULL, "
            + "typeauthentication ENUM('LDAP', 'Local') NOT NULL DEFAULT 'Local', "
            + "enabled BOOLEAN NOT NULL DEFAULT 1, "
            + "PRIMARY KEY (iduser))"
            + "ENGINE = InnoDB"));

        SQLList.Add(ReturnSQLINArray("menugrouptab", "Table",
            "CREATE  TABLE IF NOT EXISTS " + txtDBName.Text + ".menugrouptab ("
            + "idgroupmenu INT NOT NULL AUTO_INCREMENT, "
            + "title VARCHAR(100) NOT NULL, "
            + "idlanguage INT NOT NULL, "
            + "enabled BOOLEAN NOT NULL DEFAULT 1, "
            + "PRIMARY KEY (idgroupmenu)) "
            + "ENGINE = InnoDB"));

        SQLList.Add(ReturnSQLINArray("mediatab", "Table",
            "CREATE  TABLE IF NOT EXISTS " + txtDBName.Text + ".mediatab ("
            + "idmedia INT NOT NULL AUTO_INCREMENT, "
            + "title VARCHAR(255) NOT NULL, "
            + "originalname VARCHAR(255) NOT NULL, "
            + "filename VARCHAR(45) NOT NULL, "
            + "type ENUM('Image', 'Video', 'Flash', 'Audio', 'File') NOT NULL DEFAULT 'Image', "
            + "width INT, "
            + "height INT, "
            + "enabled BOOLEAN NOT NULL DEFAULT 0, "
            + "PRIMARY KEY (idmedia)) "
            + "ENGINE = InnoDB"));

        SQLList.Add(ReturnSQLINArray("mediagrouptab", "Table",
            "CREATE  TABLE IF NOT EXISTS " + txtDBName.Text + ".mediagrouptab ("
            + "idmediagroup INT NOT NULL AUTO_INCREMENT, "
            + "title VARCHAR(45) NOT NULL, "
            + "description LONGTEXT NOT NULL, "
            + "enabled BOOLEAN NOT NULL DEFAULT 0, "
            + "PRIMARY KEY (idmediagroup)) "
            + "ENGINE = InnoDB"));

        SQLList.Add(ReturnSQLINArray("articletab", "Table",
            "CREATE TABLE IF NOT EXISTS " + txtDBName.Text + ".articletab ("
            + "idarticle INT NOT NULL AUTO_INCREMENT, "
            + "title VARCHAR(100) NOT NULL, "
            + "description LONGTEXT NOT NULL, "
            + "idsubject INT NOT NULL, "
            + "iduser INT NOT NULL, "
            + "idlanguage INT NOT NULL, "
            + "type ENUM('Page', 'News') NOT NULL DEFAULT 'Page', "
            + "published DATETIME NOT NULL, "
            + "expires DATETIME NULL, "
            + "lastupdate DATETIME NULL, "
            + "idmainmedia INT NULL, "
            + "idmediagroup INT NULL, "
            + "enabledetails BOOLEAN NOT NULL DEFAULT 0, "
            + "enablenews BOOLEAN NOT NULL DEFAULT 0, "
            + "enabled BOOLEAN NOT NULL DEFAULT 0, "
            + "PRIMARY KEY (idarticle)) "
            + "ENGINE = InnoDB"));

        SQLList.Add(ReturnSQLINArray("tagtab", "Table",
            "CREATE  TABLE IF NOT EXISTS " + txtDBName.Text + ".tagtab ("
            + "idtag INT NOT NULL AUTO_INCREMENT, "
            + "tag VARCHAR(45) NOT NULL, "
            + "enabled BOOLEAN NOT NULL DEFAULT 1, "
            + "PRIMARY KEY (idtag)) "
            + "ENGINE = InnoDB"));

        SQLList.Add(ReturnSQLINArray("articletagtab", "Table",
            "CREATE  TABLE IF NOT EXISTS " + txtDBName.Text + ".articletagtab ("
            + "idarticle INT NOT NULL, "
            + "idtag INT NOT NULL, "
            + "PRIMARY KEY (idarticle, idtag)) "
            + "ENGINE = InnoDB"));

        SQLList.Add(ReturnSQLINArray("configtab", "Table",
            "CREATE  TABLE IF NOT EXISTS " + txtDBName.Text + ".configtab ("
            + "idconfig INT NOT NULL AUTO_INCREMENT, "
            + "title VARCHAR(255) NOT NULL, "
            + "idlogo INT NULL, "
            + "numbernews INT NOT NULL DEFAULT 5, "
            + "idsubject INT NULL, "
            + "idsubjectheader INT NULL, "
            + "idsubjectfooter INT NULL, "
            + "idlanguage INT NULL, "
            + "folderinstalled VARCHAR(255) NULL, "
            + "menustyle ENUM('Original', 'Tree', 'Top Bar', 'Side Bar') NOT NULL DEFAULT 'Original', "
            + "description LONGTEXT NOT NULL, "
            + "keywords LONGTEXT NOT NULL, "
            + "ldaphosturl VARCHAR(255), "
            + "ldapcontexts VARCHAR(255), "
            + "enabled INT NOT NULL DEFAULT 1, "
            + "PRIMARY KEY (idconfig)) "
            + "ENGINE = InnoDB"));

        SQLList.Add(ReturnSQLINArray("modulestab", "Table",
            "CREATE TABLE IF NOT EXISTS " + txtDBName.Text + ".modulestab ("
            + "idmodule INT NOT NULL AUTO_INCREMENT, "
            + "title VARCHAR(45) NOT NULL, "
            + "description VARCHAR(255) NOT NULL, "
            + "enabled BOOLEAN NOT NULL DEFAULT 0, "
            + "PRIMARY KEY (idmodule)) "
            + "ENGINE = InnoDB"));

        SQLList.Add(ReturnSQLINArray("moduleusertab", "Table",
            "CREATE TABLE IF NOT EXISTS " + txtDBName.Text + ".moduleusertab ("
            + "idmodule INT NOT NULL, "
            + "iduser INT NOT NULL, "
            + "selectdata BOOLEAN NOT NULL DEFAULT 0, "
            + "insertdata BOOLEAN NOT NULL DEFAULT 0, "
            + "updatedata BOOLEAN NOT NULL DEFAULT 0, "
            + "deletedata BOOLEAN NOT NULL DEFAULT 0, "
            + "PRIMARY KEY (idmodule, iduser)) "
            + "ENGINE = InnoDB"));

        SQLList.Add(ReturnSQLINArray("languagetab", "Table",
            "CREATE TABLE IF NOT EXISTS " + txtDBName.Text + ".languagetab ("
            + "idlanguage INT NOT NULL AUTO_INCREMENT, "
            + "title VARCHAR(45) NOT NULL, "
            + "abreviation VARCHAR(45) NOT NULL, "
            + "idmedia INT NOT NULL, "
            + "enabled BOOLEAN NOT NULL DEFAULT 1, "
            + "PRIMARY KEY (idlanguage)) "
            + "ENGINE = InnoDB"));

        SQLList.Add(ReturnSQLINArray("subjecttab", "Table",
            "CREATE TABLE IF NOT EXISTS " + txtDBName.Text + ".subjecttab ("
            + "idsubject INT NOT NULL AUTO_INCREMENT, "
            + "title VARCHAR(45) NOT NULL, "
            + "description VARCHAR(255) NOT NULL, "
            + "PRIMARY KEY (idsubject)) "
            + "ENGINE = InnoDB"));

        SQLList.Add(ReturnSQLINArray("medialisttab", "Table",
            "CREATE TABLE IF NOT EXISTS " + txtDBName.Text + ".medialisttab ("
            + "idmedia INT NOT NULL, "
            + "idmediagroup INT NOT NULL, "
            + "PRIMARY KEY (idmedia, idmediagroup)) "
            + "ENGINE = InnoDB"));

        SQLList.Add(ReturnSQLINArray("menutab", "Table",
            "CREATE TABLE IF NOT EXISTS " + txtDBName.Text + ".menutab ("
            + "idmenu INT NOT NULL AUTO_INCREMENT, "
            + "idgroupmenu INT NOT NULL, "
            + "idsubject INT NOT NULL , "
            + "title VARCHAR(45) NOT NULL, "
            + "position INT NULL, "
            + "idmenuparent INT NULL, "
            + "visible TINYINT (1) NOT NULL DEFAULT 1, "
            + "enabled TINYINT (1) NOT NULL DEFAULT 1, "
            + "PRIMARY KEY (idmenu)) "
            + "ENGINE = InnoDB"));

        /* Create indexes before creating the foreing key
            Creating index
            */
        SQLList.Add(ReturnSQLINArray("friendlyurltab", "Index",
            "CREATE INDEX idarticle_friendlyurltab_fk ON " + txtDBName.Text + ".friendlyurltab"
            + " (idarticle ASC)"));

        SQLList.Add(ReturnSQLINArray("menutab", "Index",
            "CREATE INDEX idmenuparent_menutab_fk ON " + txtDBName.Text + ".menutab"
            + " (idmenuparent ASC)"));

        SQLList.Add(ReturnSQLINArray("articletab", "Index",
            "CREATE INDEX idsubject_articletab_fk ON " + txtDBName.Text + ".articletab"
            + " (idsubject ASC)"));

        SQLList.Add(ReturnSQLINArray("articletab", "Index",
            "CREATE INDEX iduser_articletab_fk ON " + txtDBName.Text + ".articletab"
            + " (iduser ASC)"));

        SQLList.Add(ReturnSQLINArray("articletab", "Index",
            "CREATE INDEX idmainmedia_articletab_fk ON " + txtDBName.Text + ".articletab"
            + " (idmainmedia ASC)"));

        SQLList.Add(ReturnSQLINArray("articletab", "Index",
            "CREATE INDEX idmediagroup_articletab_fk ON " + txtDBName.Text + ".articletab"
            + " (idmediagroup ASC)"));

        SQLList.Add(ReturnSQLINArray("articletagtab", "Index",
            "CREATE INDEX idarticle_articletagtab_fk ON " + txtDBName.Text + ".articletagtab"
            + " (idarticle ASC)"));

        SQLList.Add(ReturnSQLINArray("articletagtab", "Index",
            "CREATE INDEX idtag_articletagtab_fk ON " + txtDBName.Text + ".articletagtab"
            + " (idtag ASC)"));

        SQLList.Add(ReturnSQLINArray("configtab", "Index",
            "CREATE INDEX idsubject_configtab_fk ON " + txtDBName.Text + ".configtab"
            + " (idsubject ASC)"));

        SQLList.Add(ReturnSQLINArray("configtab", "Index",
            "CREATE INDEX idmedia_configtab_fk ON " + txtDBName.Text + ".configtab"
            + " (idlogo ASC)"));

        SQLList.Add(ReturnSQLINArray("configtab", "Index",
            "CREATE INDEX idsubjectheader_configtab_fk ON " + txtDBName.Text + ".configtab"
            + " (idsubjectheader ASC)"));

        SQLList.Add(ReturnSQLINArray("configtab", "Index",
            "CREATE INDEX idsubjectfooter_configtab_fk ON " + txtDBName.Text + ".configtab"
            + " (idsubjectfooter ASC)"));

        SQLList.Add(ReturnSQLINArray("moduleusertab", "Index",
            "CREATE INDEX idmodule_moduleusertab_fk ON " + txtDBName.Text + ".moduleusertab"
            + " (idmodule ASC)"));

        SQLList.Add(ReturnSQLINArray("moduleusertab", "Index",
            "CREATE INDEX iduser_moduleusertab_fk ON " + txtDBName.Text + ".moduleusertab"
            + " (iduser ASC)"));

        SQLList.Add(ReturnSQLINArray("languagetab", "Index",
            "CREATE INDEX idmedia_languagetab_fk ON " + txtDBName.Text + ".languagetab"
            + " (idmedia ASC)"));

        SQLList.Add(ReturnSQLINArray("articletab", "Index",
            "CREATE INDEX idlanguage_articletab_fk ON " + txtDBName.Text + ".articletab"
            + " (idlanguage ASC)"));

        SQLList.Add(ReturnSQLINArray("configtab", "Index",
            "CREATE INDEX idlanguage_configtab_fk ON " + txtDBName.Text + ".configtab"
            + " (idlanguage ASC)"));

        SQLList.Add(ReturnSQLINArray("medialisttab", "Index",
            "CREATE INDEX idmedia_medialisttab_fk ON " + txtDBName.Text + ".medialisttab"
            + " (idmedia ASC)"));

        SQLList.Add(ReturnSQLINArray("medialisttab", "Index",
            "CREATE INDEX idmediagroup_medialisttab_fk ON " + txtDBName.Text + ".medialisttab"
            + " (idmediagroup ASC)"));

        SQLList.Add(ReturnSQLINArray("menutab", "Index",
            "CREATE INDEX idgroupmenu_menutab_fk ON " + txtDBName.Text + ".menutab"
            + " (idgroupmenu ASC)"));

        SQLList.Add(ReturnSQLINArray("menugrouptab", "Index",
            "CREATE INDEX idlanguage_menutab_fk ON " + txtDBName.Text + ".menugrouptab"
            + " (idlanguage ASC)"));

        SQLList.Add(ReturnSQLINArray("menutab", "Index",
            "CREATE INDEX idsubject_menutab_fk ON " + txtDBName.Text + ".menutab"
            + " (idsubject ASC)"));

    
        /***********  For Mysql server 5.5 You have to create the indexces first before you create the Foreing key
         * This was the problem
         * creating FOREIGN KEYS
         */

        SQLList.Add(ReturnSQLINArray("friendlyurltab", "References",
            "ALTER TABLE " + txtDBName.Text + ".friendlyurltab ADD "
            + "CONSTRAINT idarticle_friendlyurltab_fk "
            + "FOREIGN KEY (idarticle) "
            + "REFERENCES " + txtDBName.Text + ".articletab (idarticle) "
            + "ON DELETE NO ACTION "
            + "ON UPDATE NO ACTION "));

        SQLList.Add(ReturnSQLINArray("menutab", "References",
            "ALTER TABLE " + txtDBName.Text + ".menutab ADD "
            + "CONSTRAINT idmenuparent_menutab_fk "
            + "FOREIGN KEY (idmenuparent) "
            + "REFERENCES " + txtDBName.Text + ".menutab (idmenu) "
            + "ON DELETE NO ACTION "
            + "ON UPDATE NO ACTION, ADD "
            + "CONSTRAINT idgroupmenu_menutab_fk "
            + "FOREIGN KEY (idgroupmenu) "
            + "REFERENCES " + txtDBName.Text + ".menugrouptab (idgroupmenu) "
            + "ON DELETE NO ACTION "
            + "ON UPDATE NO ACTION, ADD "
            + "CONSTRAINT idsubject_menutab_fk "
            + "FOREIGN KEY (idsubject) "
            + "REFERENCES " + txtDBName.Text + ".subjecttab (idsubject) "
            + "ON DELETE NO ACTION "
            + "ON UPDATE NO ACTION "));

        SQLList.Add(ReturnSQLINArray("articletab", "References",
            "ALTER TABLE " + txtDBName.Text + ".articletab ADD "
            + "CONSTRAINT iduser_articletab_fk "
            + "FOREIGN KEY (iduser) "
            + "REFERENCES " + txtDBName.Text + ".usertab (iduser) "
            + "ON DELETE NO ACTION "
            + "ON UPDATE NO ACTION, ADD "
            + "CONSTRAINT idmainmedia_articletab_fk "
            + "FOREIGN KEY (idmainmedia) "
            + "REFERENCES " + txtDBName.Text + ".mediatab (idmedia) "
            + "ON DELETE NO ACTION "
            + "ON UPDATE NO ACTION, ADD "
            + "CONSTRAINT idmediagroup_articletab_fk "
            + "FOREIGN KEY (idmediagroup) "
            + "REFERENCES " + txtDBName.Text + ".mediagrouptab (idmediagroup) "
            + "ON DELETE NO ACTION "
            + "ON UPDATE NO ACTION, ADD "
            + "CONSTRAINT idsubject_articletab_fk "
            + "FOREIGN KEY (idsubject) "
            + "REFERENCES " + txtDBName.Text + ".subjecttab (idsubject) "
            + "ON DELETE NO ACTION "
            + "ON UPDATE NO ACTION, ADD "
            + "CONSTRAINT idlanguage_articletab_fk "
            + "FOREIGN KEY (idlanguage) "
            + "REFERENCES " + txtDBName.Text + ".languagetab (idlanguage) "
            + "ON DELETE NO ACTION "
            + "ON UPDATE NO ACTION "));

        SQLList.Add(ReturnSQLINArray("articletagtab", "References",
            "ALTER TABLE " + txtDBName.Text + ".articletagtab ADD "
            + "CONSTRAINT idarticle_articletagtab_fk "
            + "FOREIGN KEY (idarticle) "
            + "REFERENCES " + txtDBName.Text + ".articletab (idarticle) "
            + "ON DELETE NO ACTION "
            + "ON UPDATE NO ACTION, ADD "
            + "CONSTRAINT idtag_articletagtab_fk "
            + "FOREIGN KEY (idtag) "
            + "REFERENCES " + txtDBName.Text + ".tagtab (idtag) "
            + "ON DELETE NO ACTION "
            + "ON UPDATE NO ACTION "));

        SQLList.Add(ReturnSQLINArray("configtab", "References",
            "ALTER TABLE " + txtDBName.Text + ".configtab ADD "
            + "CONSTRAINT idsubject_configtab_fk "
            + "FOREIGN KEY (idsubject) "
            + "  REFERENCES " + txtDBName.Text + ".subjecttab (idsubject) "
            + "  ON DELETE NO ACTION "
            + "  ON UPDATE NO ACTION, ADD "
            + "CONSTRAINT idmedia_configtab_fk "
            + "  FOREIGN KEY (idlogo) "
            + "  REFERENCES " + txtDBName.Text + ".mediatab (idmedia) "
            + "  ON DELETE NO ACTION "
            + "  ON UPDATE NO ACTION, ADD "
            + "CONSTRAINT idsubjectheader_configtab_fk "
            + "  FOREIGN KEY (idsubjectheader) "
            + "  REFERENCES " + txtDBName.Text + ".subjecttab (idsubject) "
            + "  ON DELETE NO ACTION "
            + "  ON UPDATE NO ACTION, ADD "
            + "CONSTRAINT idsubjectfooter_configtab_fk "
            + "  FOREIGN KEY (idsubjectfooter) "
            + "  REFERENCES " + txtDBName.Text + ".subjecttab (idsubject) "
            + "  ON DELETE NO ACTION "
            + "  ON UPDATE NO ACTION, ADD "
            + "CONSTRAINT idlanguage_configtab_fk "
            + "  FOREIGN KEY (idlanguage) "
            + "  REFERENCES " + txtDBName.Text + ".languagetab (idlanguage) "
            + "  ON DELETE NO ACTION "
            + "  ON UPDATE NO ACTION "));

        SQLList.Add(ReturnSQLINArray("moduleusertab", "References",
            "ALTER TABLE " + txtDBName.Text + ".moduleusertab ADD "
            + "CONSTRAINT idmodule_moduleusertab_fk "
            + "FOREIGN KEY (idmodule) "
            + "REFERENCES " + txtDBName.Text + ".modulestab (idmodule) "
            + "ON DELETE NO ACTION "
            + "ON UPDATE NO ACTION, ADD "
            + "CONSTRAINT iduser_moduleusertab_fk "
            + "FOREIGN KEY (iduser) "
            + "REFERENCES " + txtDBName.Text + ".usertab (iduser) "
            + "ON DELETE NO ACTION "
            + "ON UPDATE NO ACTION "));
        
        SQLList.Add(ReturnSQLINArray("languagetab", "References",
            "ALTER TABLE " + txtDBName.Text + ".languagetab ADD "
            + "CONSTRAINT idmedia_languagetab_fk "
            + "FOREIGN KEY (idmedia) "
            + "REFERENCES " + txtDBName.Text + ".mediatab (idmedia) "
            + "ON DELETE NO ACTION "
            + "ON UPDATE NO ACTION "));

        SQLList.Add(ReturnSQLINArray("medialisttab", "References",
            "ALTER TABLE " + txtDBName.Text + ".medialisttab ADD "
            + "CONSTRAINT idmedia_medialisttab_fk "
            + "FOREIGN KEY (idmedia) "
            + "REFERENCES " + txtDBName.Text + ".mediatab (idmedia) "
            + "ON DELETE NO ACTION "
            + "ON UPDATE NO ACTION, ADD "
            + "CONSTRAINT idmediagroup_medialisttab_fk "
            + "FOREIGN KEY (idmediagroup) "
            + "REFERENCES " + txtDBName.Text + ".mediagrouptab (idmediagroup) "
            + "ON DELETE NO ACTION "
            + "ON UPDATE NO ACTION "));

        SQLList.Add(ReturnSQLINArray("menugrouptab", "References",
            "ALTER TABLE " + txtDBName.Text + ".menugrouptab ADD "
            + "CONSTRAINT idlanguage_menutab_fk "
            + "FOREIGN KEY (idlanguage) "
            + "REFERENCES " + txtDBName.Text + ".languagetab (idlanguage) "
            + "ON DELETE NO ACTION "
            + "ON UPDATE NO ACTION "));


            

        /*POPULATING DATA*/

        SQLList.Add(ReturnSQLINArray("mediatab", "Datas",
            "INSERT INTO " + txtDBName.Text + ".mediatab"
            + " (title, originalname, filename, type, width, height, enabled) "
            + "VALUES ('Logo John Mendes', 'JohnMendesLogo.gif', '1.gif', 'Image', 290, 50, 1)"));

        SQLList.Add(ReturnSQLINArray("mediatab", "Datas", 
            "INSERT INTO " + txtDBName.Text + ".mediatab"
            + " (title, originalname, filename, type, width, height, enabled) "
            + "VALUES ('UK Flag', 'EnglishFlag.png', '2.png', 'Image', 16, 16, 1)"));

        SQLList.Add(ReturnSQLINArray("usertab", "Datas", 
            "INSERT INTO " + txtDBName.Text + ".usertab"
            + " (name, surname, password, email, username, enabled, date, type) "
            + "VALUES ('Administrator', 'Manager', '21232f297a57a5a743894a0e4a801fc3', 'johnafmendes@gmail.com', "
            + "'admin', 1, '" 
            + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " " + DateTime.Now.Hour + ":" 
            + DateTime.Now.Minute + ":" + DateTime.Now.Second + "', 'Admin')"));

        SQLList.Add(ReturnSQLINArray("versiontab", "Datas",
            "INSERT INTO " + txtDBName.Text + ".versiontab"
            + " (currentversiondesc, CurrentVersionNum, dateinstallation) VALUES ('3.0.0 - 17/06/2009', 3, '" 
            + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " " + DateTime.Now.Hour 
            + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "')"));

        SQLList.Add(ReturnSQLINArray("languagetab", "Datas",
            "INSERT INTO " + txtDBName.Text + ".languagetab"
            + " (title, abreviation, idmedia, enabled) VALUES ('English', 'en-gb', 2, 1)"));

        SQLList.Add(ReturnSQLINArray("configtab", "Datas",
            "INSERT INTO " + txtDBName.Text + ".configtab"
            + " (idconfig, title, idlogo, numbernews, idsubject, idsubjectheader, idsubjectfooter, idlanguage, "
            + "folderinstalled, menustyle, description, keywords, enabled) "
            + "VALUES (0, 'John Mendes CMS Web Site', 1, 5, NULL, NULL, NULL, 1, '', 'Original', '', '', 0)"));

        SQLList.Add(ReturnSQLINArray("modulestab", "Datas",
            "INSERT INTO " + txtDBName.Text + ".modulestab"
            + " (title, description) VALUES ('Users', 'Users Management')"));

        SQLList.Add(ReturnSQLINArray("modulestab", "Datas",
            "INSERT INTO " + txtDBName.Text + ".modulestab"
            + " (title, description) VALUES ('Menus', 'Menus Management')"));

        SQLList.Add(ReturnSQLINArray("modulestab", "Datas",
            "INSERT INTO " + txtDBName.Text + ".modulestab"
            + " (title, description) VALUES ('Media', 'Media Management')"));

        SQLList.Add(ReturnSQLINArray("modulestab", "Datas",
            "INSERT INTO " + txtDBName.Text + ".modulestab"
            + " (title, description) VALUES ('Articles', 'Articles Management')"));

        SQLList.Add(ReturnSQLINArray("modulestab", "Datas",
            "INSERT INTO " + txtDBName.Text + ".modulestab"
            + " (title, description) VALUES ('Tags', 'Tags Management')"));

        SQLList.Add(ReturnSQLINArray("modulestab", "Datas",
                "INSERT INTO " + txtDBName.Text + ".modulestab"
            + " (title, description) VALUES ('Settings', 'Settings Management')"));

        SQLList.Add(ReturnSQLINArray("modulestab", "Datas",
            "INSERT INTO " + txtDBName.Text + ".modulestab"
            + " (title, description) VALUES ('Group Media', 'Group of Media Management')"));

        SQLList.Add(ReturnSQLINArray("modulestab", "Datas",
            "INSERT INTO " + txtDBName.Text + ".modulestab"
            + " (title, description) VALUES ('Languages', 'Languages Management')"));

        SQLList.Add(ReturnSQLINArray("modulestab", "Datas",
            "INSERT INTO " + txtDBName.Text + ".modulestab"
            + " (title, description) VALUES ('Group Menu', 'Group of Menu Management')"));

        SQLList.Add(ReturnSQLINArray("modulestab", "Datas",
            "INSERT INTO " + txtDBName.Text + ".modulestab"
            + " (title, description) VALUES ('Subject', 'Subjects Management')"));

        SQLList.Add(ReturnSQLINArray("modulestab", "Datas",
            "INSERT INTO " + txtDBName.Text + ".modulestab"
            + " (title, description) VALUES ('Upgrade', 'Upgrade System')"));

        SQLList.Add(ReturnSQLINArray("moduleusertab", "Datas",
            "INSERT INTO " + txtDBName.Text + ".moduleusertab"
            + " (idmodule, iduser, selectdata, updatedata, insertdata, deletedata) VALUES (1, 1, 1, 1, 1, 1)"));

        SQLList.Add(ReturnSQLINArray("moduleusertab", "Datas",
            "INSERT INTO " + txtDBName.Text + ".moduleusertab"
            + " (idmodule, iduser, selectdata, updatedata, insertdata, deletedata) VALUES (2, 1, 1, 1, 1, 1)"));

        SQLList.Add(ReturnSQLINArray("moduleusertab", "Datas",
            "INSERT INTO " + txtDBName.Text + ".moduleusertab"
            + " (idmodule, iduser, selectdata, updatedata, insertdata, deletedata) VALUES (3, 1, 1, 1, 1, 1)"));

        SQLList.Add(ReturnSQLINArray("moduleusertab", "Datas",
            "INSERT INTO " + txtDBName.Text + ".moduleusertab"
            + " (idmodule, iduser, selectdata, updatedata, insertdata, deletedata) VALUES (4, 1, 1, 1, 1, 1)"));

        SQLList.Add(ReturnSQLINArray("moduleusertab", "Datas",
            "INSERT INTO " + txtDBName.Text + ".moduleusertab"
            + " (idmodule, iduser, selectdata, updatedata, insertdata, deletedata) VALUES (5, 1, 1, 1, 1, 1)"));

        SQLList.Add(ReturnSQLINArray("moduleusertab", "Datas",
            "INSERT INTO " + txtDBName.Text + ".moduleusertab"
            + " (idmodule, iduser, selectdata, updatedata, insertdata, deletedata) VALUES (6, 1, 1, 1, 1, 1)"));

        SQLList.Add(ReturnSQLINArray("moduleusertab", "Datas",
            "INSERT INTO " + txtDBName.Text + ".moduleusertab"
            + " (idmodule, iduser, selectdata, updatedata, insertdata, deletedata) VALUES (7, 1, 1, 1, 1, 1)"));

        SQLList.Add(ReturnSQLINArray("moduleusertab", "Datas",
            "INSERT INTO " + txtDBName.Text + ".moduleusertab"
            + " (idmodule, iduser, selectdata, updatedata, insertdata, deletedata) VALUES (8, 1, 1, 1, 1, 1)"));

        SQLList.Add(ReturnSQLINArray("moduleusertab", "Datas",
            "INSERT INTO " + txtDBName.Text + ".moduleusertab"
            + " (idmodule, iduser, selectdata, updatedata, insertdata, deletedata) VALUES (9, 1, 1, 1, 1, 1)"));

        SQLList.Add(ReturnSQLINArray("moduleusertab", "Datas",
            "INSERT INTO " + txtDBName.Text + ".moduleusertab"
            + " (idmodule, iduser, selectdata, updatedata, insertdata, deletedata) VALUES (10, 1, 1, 1, 1, 1)"));

        SQLList.Add(ReturnSQLINArray("moduleusertab", "Datas",
            "INSERT INTO " + txtDBName.Text + ".moduleusertab"
            + " (idmodule, iduser, selectdata, updatedata, insertdata, deletedata) VALUES (11, 1, 1, 1, 1, 1)"));

        try
        {
            //SQLList.Reverse();
            foreach(string[,] _SQL in SQLList)
            {
                cmd.CommandText = _SQL[0, 0];

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (OdbcException o)
                {
                    tmsg = "We are sorry, we are experiencing technical problems ...";
                    tmsg += "<hr/>";
                    tmsg += "<br/>Message: " + o.Message + " SQL: " + _SQL[0, 0];
                    tmsg += "<br/>Source: " + o.Source;
                    tmsg += "<br/>Stack Trace: " + o.StackTrace;
                    tmsg += "<br/>Target Site: " + o.TargetSite.Name;
                    tmsg += "<br/>Others: " + o.InnerException;
                    tmsg += "<hr/>";
                    tmsg += "<br/>Try again! If the problem persist, contact techinical suport";
                }
                finally
                {
                    //cmd.Dispose();
                    if (tmsg.Length == 0)
                    {
                        msg.InnerHtml += _SQL[0, 2].ToString() + ": " + _SQL[0, 1].ToString() 
                            + " created successfully!<br/>";
                        //createIndex(sender, e);
                    }
                    else msg.InnerHtml = tmsg;
                }
            }//end for
        }
        catch (Exception ex)
        {
            tmsg = "We are sorry, we are experiencing technical problems ...";
            tmsg += "<hr/>";
            tmsg += "<br/>Message: " + ex.Message;
            tmsg += "<br/>Source: " + ex.Source;
            tmsg += "<br/>Stack Trace: " + ex.StackTrace;
            tmsg += "<br/>Target Site: " + ex.TargetSite.Name;
            tmsg += "<br/>Others: " + ex.InnerException;
            tmsg += "<hr/>";
            tmsg += "<br/>Try again! If the problem persist, contact techinical suport";
        }
        finally
        {
            //cmd.Dispose();
            if (tmsg.Length == 0)
            {
                result = true;
            }
            else result = false;
        }
        cmd.Dispose();
        return result;
    }
}
