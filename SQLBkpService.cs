using System;
using System.Data.SqlClient;
using System.ServiceProcess;
using System.Timers;
using SQLBkpService.classes;


namespace SQLBkpService
{
    public partial class SQLBkpService : ServiceBase
    {
        public Timer timer = new Timer();
        public Utils utils = new Utils();
        public SQLBkpService()
        {
            InitializeComponent();
            
        }

        protected override void OnStart(string[] args)
        {
            utils.WriteReport("Se ha Inicializando el servicio");
            timer.Elapsed += new ElapsedEventHandler(OnTimer);
            timer.Interval = 600000;
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            utils.WriteReport("Se ha Detenido el servicio");
            timer.Stop();
        }

        protected void OnTimer(object sender, ElapsedEventArgs e)
        {
            //instancias y Variables
            Settings settings = utils.LoadSettings();
            string secretKey = "YourSecretKey";
            SqlConnection connection;
            connection = utils.getConnection(settings, secretKey);
            int hour = DateTime.Now.Hour;

            utils.WriteReport("Monitoreando ...");
            try
            {

                if (hour == settings.activeHour)
                {
                    utils.WriteReport("Iniciando operaciones");
                    utils.WriteReport("Obteniendo conexión al servidor");

                    //revisar si esta el backup
                    if (!utils.checkBackup(settings))
                    {
                        utils.WriteReport("Iniciando respaldo ...");

                        //ejecutar query para los respaldos
                        utils.BackupDatabases(connection,settings,secretKey);

                        //comprimir los archivos bak y ponerlos en onedrive.
                        utils.CompressBaks(settings);

                        //limpiar los baks residuales de la compresion
                        utils.EraseBAKS(settings.srcFolder);
                        
                    }

                }
            }
            catch (Exception ex)
            {
                utils.WriteReport(ex.Message);
            }
            finally
            {
                connection.Close();
                Dispose();
            }
            utils.WriteReport("En espera ...");

        }
    }
}
