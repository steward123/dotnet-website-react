using DataAPI.Configs;
using DataAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;

namespace DataAPI.Services
{
    public class PortfolioServices
    {
        private readonly IMongoCollection<contact> _contactCollection;
        private readonly IMongoCollection<ResumeDownload> _resumeCollection;

        public PortfolioServices(IOptions<PortfolioDatabaseSettings> portfolioDatabaseSettings) 
        {
            var mongoclient = new MongoClient(portfolioDatabaseSettings.Value.ConnectionString);

            var databaseName = mongoclient.GetDatabase(portfolioDatabaseSettings.Value.DatabaseName);

            //_contactCollection = databaseName.GetCollection<contact>(portfolioDatabaseSettings.Value.CollectionName);
            _contactCollection = databaseName.GetCollection<contact>(PortfolioClassesNames.className_contact);

            //resume-download-collection
            _resumeCollection = databaseName.GetCollection<ResumeDownload>(PortfolioClassesNames.className_resumeDownload);
        }

        public async Task<contact> createUser(contact contact)
        {
            await _contactCollection.InsertOneAsync(contact);
            return (contact);
        }

        public async Task<List<contact>> getContactsAsync()
        {
           return await _contactCollection.Find(_ =>true).ToListAsync();
        }

        public async Task<contact> GetContact(string Id)
        {
            return await _contactCollection.Find(x => x.Id == Id).FirstOrDefaultAsync();
        }

        public async Task<ResumeDownload> getResume(ResumeDownload resumeDownload)
        {
            contact obj = await _contactCollection.Find(x => x.Name == resumeDownload.contact.Name).FirstOrDefaultAsync();
            
            if (obj != null)
            {
                // check number of days is greater than 30 or not
                // if it is not greater throw an error else success
                DateTime d = DateTime.UtcNow.Date;
                var diff = Convert.ToInt32((resumeDownload.LastModified - d));
                if (diff > 30)
                {
                    var filter = Builders<ResumeDownload>.Filter.Eq(t => t.contact.Name, obj.Name);
                    var update = Builders<ResumeDownload>.Update.Set(x => x.LastModified, d).Set(x => x.guid_generated, Guid.NewGuid().ToString());
                    await _resumeCollection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = false });
                }
            }
            else
            {
                await _contactCollection.InsertOneAsync(resumeDownload.contact);
                var guid = Guid.NewGuid().ToString();
                resumeDownload.guid_generated = guid;
                DateTime dateTime = DateTime.UtcNow.Date;
                resumeDownload.LastModified = dateTime;
                await _resumeCollection.InsertOneAsync(resumeDownload);
            }

            return resumeDownload;

        }
    }
}
