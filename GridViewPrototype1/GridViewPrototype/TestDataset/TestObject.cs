using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestDataset
{
    public class TestObject
    {

        public String Name { get; set; }
        public String VariableType{ get; set; }
        public String Description { get; set; }
        public String Scope { get; set; }
        public String Dimension { get; set; }
        public UInt32 ID { get; set; }

    }

    public class ObjectList
    {
        private List<TestObject> m_testObjects;
        public ObjectList()
        {
            m_testObjects = new List<TestObject>();
            m_testObjects.Add(new TestObject()
            {
                Name = "var1",
                VariableType = VariableTypeString.BOOL,
                Description = "Variable1",
                Scope = "LocalVariable",
                Dimension = "[1..4]",
                ID = 1
            });
            m_testObjects.Add(new TestObject()
            {
                Name = "var2",
                VariableType = VariableTypeString.BOOL,
                Description = "Variable2",
                Scope = "LocalVariable",
                Dimension = "[1..4]",
                ID = 2
            });

        }

        public List<TestObject> GetListOfObjects()
        {
            return m_testObjects;
        }
    }
}
