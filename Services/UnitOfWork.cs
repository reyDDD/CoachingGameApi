﻿using TamboliyaApi.Data;

namespace TamboliyaApi.Services
{
    public class UnitOfWork : IDisposable
    {
        private readonly AppDbContext context = null!;

        private GenericRepository<Game> gameRepository = null!;
        private GenericRepository<InitialGameData> oracleRepository = null!;
        private GenericRepository<ActualPositionOnTheMap> actualPositionOnTheMapRepository = null!;
        private GenericRepository<ActualPositionsOnMapForSelect> actualPositionsOnMapForSelectRepository = null!;




        public UnitOfWork(AppDbContext context)
        {
            this.context = context;
        }

        public GenericRepository<Game> GameRepository
        {
            get
            {

                if (this.gameRepository == null)
                {
                    this.gameRepository = new GenericRepository<Game>(context);
                }
                return gameRepository;
            }
        }

        public GenericRepository<InitialGameData> OracleRepository
        {
            get
            {

                if (this.oracleRepository == null)
                {
                    this.oracleRepository = new GenericRepository<InitialGameData>(context);
                }
                return oracleRepository;
            }
        }

        public GenericRepository<ActualPositionOnTheMap> ActualPositionOnTheMapRepository
        {
            get
            {

                if (this.actualPositionOnTheMapRepository == null)
                {
                    this.actualPositionOnTheMapRepository = new GenericRepository<ActualPositionOnTheMap>(context);
                }
                return actualPositionOnTheMapRepository;
            }
        }

        public GenericRepository<ActualPositionsOnMapForSelect> ActualPositionsOnMapForSelectRepository
        {
            get
            {

                if (this.actualPositionsOnMapForSelectRepository == null)
                {
                    this.actualPositionsOnMapForSelectRepository = new GenericRepository<ActualPositionsOnMapForSelect>(context);
                }
                return actualPositionsOnMapForSelectRepository;
            }
        }


        public async Task SaveAsync()
        {
           await context.SaveChangesAsync();
        }






        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
