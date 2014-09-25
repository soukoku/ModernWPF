using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ModernWPF.Mvvm.Tests
{
    [TestClass]
    public class AutoDisposeObservableCollectionTests
    {
        class MyCounter : IDisposable
        {
            public static int Count { get; set; }

            #region IDisposable Members

            public void Dispose()
            {
                Count++;
            }

            #endregion
        }


        [TestMethod]
        public void Clear_Calls_Dispose_On_All_Items()
        {
            MyCounter.Count = 0;

            var coll = new AutoDisposeObservableCollection<MyCounter>();
            for (int i = 0; i < 10; i++)
            {
                coll.Add(new MyCounter());
            }
            coll.Clear();

            Assert.AreEqual(10, MyCounter.Count);
        }

        [TestMethod]
        public void Remove_By_Index_Calls_Dispose()
        {
            MyCounter.Count = 0;

            var coll = new AutoDisposeObservableCollection<MyCounter>();
            for (int i = 0; i < 10; i++)
            {
                coll.Add(new MyCounter());
            }
            coll.RemoveAt(3);

            Assert.AreEqual(1, MyCounter.Count);
        }

        [TestMethod]
        public void Remove_By_Object_Calls_Dispose()
        {
            MyCounter.Count = 0;

            var coll = new AutoDisposeObservableCollection<MyCounter>();
            for (int i = 0; i < 10; i++)
            {
                coll.Add(new MyCounter());
            }
            coll.Remove(coll[8]);
            Assert.AreEqual(1, MyCounter.Count);
        }


        [TestMethod]
        public void Replace_By_Index_Calls_Dispose()
        {
            MyCounter.Count = 0;

            var coll = new AutoDisposeObservableCollection<MyCounter>();
            for (int i = 0; i < 10; i++)
            {
                coll.Add(new MyCounter());
            }
            coll[5] = new MyCounter();
            Assert.AreEqual(1, MyCounter.Count);
        }
    }
}
