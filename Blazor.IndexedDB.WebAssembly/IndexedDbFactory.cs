using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;
using TG.Blazor.IndexedDB;

namespace Blazor.IndexedDB.WebAssembly
{
    public class IndexedDbFactory : IIndexedDbFactory
    {
        private readonly IJSRuntime jSRuntime;

        public IndexedDbFactory(IJSRuntime jSRuntime)
        {
            this.jSRuntime = jSRuntime;
        }

        /// <summary>
        /// Creates a new instance of the given indexed db type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Task<T> Create<T>() where T : IndexedDb => Create<T>(name: typeof(T).Name, version: 1);

        /// <summary>
        /// Creates a new instance of the given indexed db type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="version">IndexedDb version</param>
        /// <returns></returns>
        public Task<T> Create<T>(int version) where T : IndexedDb => Create<T>(name: typeof(T).Name, version: version);

        /// <summary>
        /// Creates a new instance of the given indexed db type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">IndexedDb name</param>
        /// <returns></returns>
        public Task<T> Create<T>(string name) where T : IndexedDb => Create<T>(name: name, version: 1);

        /// <summary>
        /// Creates a new instance of the given indexed db type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">IndexedDb name</param>
        /// <param name="name">IndexedDb version</param>
        /// <returns></returns>
        public async Task<T> Create<T>(string name, int version) where T : IndexedDb
        {
            var instance = (T)Activator.CreateInstance(typeof(T), this.jSRuntime, name, version);

            var connected = await instance.WaitForConnection();

            if (!connected)
            {
                throw new Exception("Could not connect");
            }

            return instance;
        }


        public Task Delete<T>() where T : IndexedDb => Delete(name: typeof(T).Name);
        public Task Delete<T>(string name) where T : IndexedDb => Delete(name: name);
        public async Task Delete(string name)
        {
            var manager = new IndexedDBManager(new DbStore{ }, jSRuntime);
            await manager.DeleteDb(name);
        }
    }
}
