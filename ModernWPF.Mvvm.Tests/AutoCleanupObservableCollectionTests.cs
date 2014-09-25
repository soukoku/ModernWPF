using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GalaSoft.MvvmLight;

namespace ModernWPF.Mvvm.Tests
{
    [TestClass]
    public class AutoCleanupObservableCollectionTests
    {
        class MyCounter : ICleanup
        {
            public static int Count { get; set; }


            #region ICleanup Members

            public void Cleanup()
            {
                Count++;
            }

            #endregion
        }


        [TestMethod]
        public void Clear_Calls_Cleanup_On_All_Items()
        {
            MyCounter.Count = 0;

            var coll = new AutoCleanupObservableCollection<MyCounter>();
            for (int i = 0; i < 10; i++)
            {
                coll.Add(new MyCounter());
            }
            coll.Clear();

            Assert.AreEqual(10, MyCounter.Count);
        }

        [TestMethod]
        public void Remove_By_Index_Calls_Cleanup()
        {
            MyCounter.Count = 0;

            var coll = new AutoCleanupObservableCollection<MyCounter>();
            for (int i = 0; i < 10; i++)
            {
                coll.Add(new MyCounter());
            }
            coll.RemoveAt(3);

            Assert.AreEqual(1, MyCounter.Count);
        }

        [TestMethod]
        public void Remove_By_Object_Calls_Cleanup()
        {
            MyCounter.Count = 0;

            var coll = new AutoCleanupObservableCollection<MyCounter>();
            for (int i = 0; i < 10; i++)
            {
                coll.Add(new MyCounter());
            }
            coll.Remove(coll[8]);
            Assert.AreEqual(1, MyCounter.Count);
        }


        [TestMethod]
        public void Replace_By_Index_Calls_Cleanup()
        {
            MyCounter.Count = 0;

            var coll = new AutoCleanupObservableCollection<MyCounter>();
            for (int i = 0; i < 10; i++)
            {
                coll.Add(new MyCounter());
            }
            coll[5] = new MyCounter();
            Assert.AreEqual(1, MyCounter.Count);
        }
    }
}
