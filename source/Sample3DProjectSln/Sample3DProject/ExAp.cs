using GenFusionsRevitCore.Servers3dContext;
using Nice3point.Revit.Toolkit.External;
using Sample3DProject.Commands;
using Serilog;
using Serilog.Events;

namespace Sample3DProject
{
    /// <summary>
    ///     Application entry point
    /// </summary>
    [UsedImplicitly]
    public class ExAp : ExternalApplication
    {
        public ServerStateMachine ServerStateMachine { get; private set; }
        public static ExAp appInstance { get; private set; }

        public override void OnStartup()
        {
            appInstance = this;
            this.ServerStateMachine = new ServerStateMachine(this);

            CreateLogger();
            CreateRibbon();
        }

        public override void OnShutdown()
        {
            Log.CloseAndFlush();
        }

        private void CreateRibbon()
        {
            var panel_Draw = Application.CreatePanel("Draw", "Sample3DProject");
            var panel_Clear = Application.CreatePanel("Clear", "Sample3DProject");


            //-- Panel Draw
            panel_Draw.AddPushButton<DrawPointWithSphere>("Point\nSpere")
                .SetImage("/Sample3DProject;component/Resources/Icons/iconGreen.png")
                .SetLargeImage("/Sample3DProject;component/Icons/iconGreen.png");

            panel_Draw.AddSeparator();

            panel_Draw.AddPushButton<DrawPointsWithSpheres>("Points\nSperes")
                .SetImage("/Sample3DProject;component/Resources/Icons/iconGreen.png")
                .SetLargeImage("/Sample3DProject;component/Icons/iconGreen.png");

            panel_Draw.AddSeparator();

            panel_Draw.AddPushButton<DrawPointWithCube>("Point\nCube")
                .SetImage("/Sample3DProject;component/Resources/Icons/iconGreen.png")
                .SetLargeImage("/Sample3DProject;component/Icons/iconGreen.png");

            panel_Draw.AddSeparator();

            panel_Draw.AddPushButton<DrawPointsWithCubes>("Points\nCubes")
                .SetImage("/Sample3DProject;component/Resources/Icons/iconGreen.png")
                .SetLargeImage("/Sample3DProject;component/Icons/iconGreen.png");

            panel_Draw.AddSeparator();

            panel_Draw.AddPushButton<DrawDirection>("Blend\n(Direction)")
                .SetImage("/Sample3DProject;component/Resources/Icons/iconGreen.png")
                .SetLargeImage("/Sample3DProject;component/Icons/iconGreen.png");

            panel_Draw.AddSeparator();

            panel_Draw.AddPushButton<DrawSolidsOfWalls>("Wall\nSolids")
                .SetImage("/Sample3DProject;component/Resources/Icons/iconGreen.png")
                .SetLargeImage("/Sample3DProject;component/Icons/iconGreen.png");

            //-- Panel Celar
            panel_Clear.AddPushButton<ClearSolidServers>("Solid\nServers")
                .SetImage("/Sample3DProject;component/Resources/Icons/iconBlue.png")
                .SetLargeImage("/Sample3DProject;component/Icons/iconBlue.png");
        }

        private static void CreateLogger()
        {
            const string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Debug(LogEventLevel.Debug, outputTemplate)
                .MinimumLevel.Debug()
                .CreateLogger();

            AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            {
                var exception = (Exception)args.ExceptionObject;
                Log.Fatal(exception, "Domain unhandled exception");
            };
        }
    }
}