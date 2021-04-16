using Microsoft.EntityFrameworkCore;

namespace PosCFG.Models{

    //connection SQL SERVER
    public class PosCFGDbContext : DbContext{
    public PosCFGDbContext(DbContextOptions<PosCFGDbContext> data)
    :base (data){}

    public DbSet<Terminal> Terminals{get; set;}

    public DbSet<SystemPOS> SystemPOSs{get; set;}

    //table TerminalsStatus
    public DbSet<TerminalStatus> TerminalsStatus{get;set;}

    public DbSet<CargaMasivaAux> CargaMasivaAux {get; set;}
            
    }

    //connection MYSQL - JPOS
    public class JPOSDbContext : DbContext{
        public JPOSDbContext(DbContextOptions<JPOSDbContext> data)
        :base (data){}

        public DbSet<Sysconfig> sysconfig{get; set;}
        
    }
}