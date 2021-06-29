using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable CA1707 // Identifiers should not contain underscores
namespace Serializer
{
	[TestClass]
	public class Tests
	{
		private static TestClass Init()
		{
			// INIT DATA
			TestClass classInst = new TestClass();
			classInst.Field1 = "test1";
			classInst.Field2 = "test1";
			classInst.Field3 = "test1";
			classInst.Field4 = "test1";
			classInst.Field5 = "test1";
			classInst.Flags1 = 10;
			classInst.Flags2 = 20;
			classInst.Flags3 = 30;
			classInst.Flags4 = 40;
			classInst.Flags5 = 50;
			classInst.TestBool = true;
			classInst.TestFloat = 1.23456f;

			classInst.TestEnum = TestEnumType.Two;

			SmallTestClass small = new SmallTestClass();
			small.SmallField = "abc small test";
			small.SubClass = new SmallTestClass2();
			small.SubClass.SmallField = "sub class";

			classInst.SmallTestClassInst = small;

			SmallTestClass small2 = new SmallTestClass();
			small2.SmallField = "another small test";
			small2.SubClass = new SmallTestClass2();
			small2.SubClass.SmallField = "sub class 2";

			classInst.SmallTestClassInst2 = small2;


			classInst.List1 = new List<int>();
			for (int i = 0; i < 10; ++i)
			{
				classInst.List1.Add(i);
			}

			classInst.List2 = new List<string>();
			for (int i = 0; i < 10; ++i)
			{
				classInst.List2.Add(String.Format(new System.Globalization.CultureInfo("en-US"), "test string {0}", i));
			}

			classInst.List3 = new List<SmallTestClass2>();
			for (int i = 0; i < 10; ++i)
			{
				SmallTestClass2 c = new SmallTestClass2();
				c.SmallField = "inner";
				classInst.List3.Add(c);
			}

			return classInst;
		}

		[TestMethod]
		public void TestSerializeAndDeserialize()
		{
			TestClass classInst = Init();

			BinaryBuffer buffer = BinarySerializer.FastSerialize(classInst);

			TestClass fastobj = BinarySerializer.FastDeserialize<TestClass>(buffer.GetBytes());
			Assert.AreEqual(classInst, fastobj);
		}

		private string m_strKnownSerializedData = "BQAAAHQAZQBzAHQAMQAFAAAAdABlAHMAdAAxAAUAAAB0AGUAcwB0ADEABQAAAHQAZQBzAHQAMQAFAAAAdABlAHMAdAAxAAoAAAAUAAAAHgAAACgAAAAyAAAACgAAAAAAAAABAAAAAgAAAAMAAAAEAAAABQAAAAYAAAAHAAAACAAAAAkAAAAKAAAADQAAAHQAZQBzAHQAIABzAHQAcgBpAG4AZwAgADAADQAAAHQAZQBzAHQAIABzAHQAcgBpAG4AZwAgADEADQAAAHQAZQBzAHQAIABzAHQAcgBpAG4AZwAgADIADQAAAHQAZQBzAHQAIABzAHQAcgBpAG4AZwAgADMADQAAAHQAZQBzAHQAIABzAHQAcgBpAG4AZwAgADQADQAAAHQAZQBzAHQAIABzAHQAcgBpAG4AZwAgADUADQAAAHQAZQBzAHQAIABzAHQAcgBpAG4AZwAgADYADQAAAHQAZQBzAHQAIABzAHQAcgBpAG4AZwAgADcADQAAAHQAZQBzAHQAIABzAHQAcgBpAG4AZwAgADgADQAAAHQAZQBzAHQAIABzAHQAcgBpAG4AZwAgADkACgAAAAUAAABpAG4AbgBlAHIABQAAAGkAbgBuAGUAcgAFAAAAaQBuAG4AZQByAAUAAABpAG4AbgBlAHIABQAAAGkAbgBuAGUAcgAFAAAAaQBuAG4AZQByAAUAAABpAG4AbgBlAHIABQAAAGkAbgBuAGUAcgAFAAAAaQBuAG4AZQByAAUAAABpAG4AbgBlAHIADgAAAGEAYgBjACAAcwBtAGEAbABsACAAdABlAHMAdAAJAAAAcwB1AGIAIABjAGwAYQBzAHMAEgAAAGEAbgBvAHQAaABlAHIAIABzAG0AYQBsAGwAIAB0AGUAcwB0AAsAAABzAHUAYgAgAGMAbABhAHMAcwAgADIAARAGnj8CAAAA";

		[TestMethod]
		public void TestSerializationOfKnownData()
		{
			TestClass classInst = Init();
			BinaryBuffer buffer = BinarySerializer.FastSerialize(classInst);
			Assert.AreEqual(m_strKnownSerializedData, Convert.ToBase64String(buffer.GetBytes()));
		}

		[TestMethod]
		public void TestDeserializationOfKnownData()
		{
			TestClass expectedInst = Init();
			TestClass fixedObj = BinarySerializer.FastDeserialize<TestClass>(Convert.FromBase64String(m_strKnownSerializedData));
			Assert.AreEqual(expectedInst, fixedObj);
		}

		[TestMethod]
		public void TestSerializeAndDeserializeList()
		{
			List<int> lstTest = new List<int>();
			lstTest.Add(1);
			lstTest.Add(2);
			lstTest.Add(3);

			BinaryBuffer buffer = BinarySerializer.FastSerialize(lstTest);

			List<int> lstComp = BinarySerializer.FastDeserialize<List<int>>(buffer.GetBytes());

			Assert.IsTrue(lstComp.SequenceEqual(lstTest));
		}

		[TestMethod]
		public void TestSerializeAndDeserializeListComplex()
		{
			List<GangTagLayer> lstTest = new List<GangTagLayer>();
			lstTest.Add(new GangTagLayer(ELayerType.Text, 0, 255, 0, 0, 255, 1.0f, 2.0f, 3.0f, "test 1", 0, true, false, 0, 1.0f, 2.0f, 3.0f));
			lstTest.Add(new GangTagLayer(ELayerType.Sprite, 0, 0, 255, 0, 200, 1.0f, 2.0f, 3.0f, "test 2", 0, true, false, 0, 1.0f, 2.0f, 3.0f));
			lstTest.Add(new GangTagLayer(ELayerType.Rectangle, 0, 0, 0, 255, 150, 1.0f, 2.0f, 3.0f, "test 3", 0, true, false, 0, 1.0f, 2.0f, 3.0f));

			BinaryBuffer buffer = BinarySerializer.FastSerialize(lstTest);

			List<GangTagLayer> lstComp = BinarySerializer.FastDeserialize<List<GangTagLayer>>(buffer.GetBytes());

			Assert.IsTrue(lstComp.SequenceEqual(lstTest));
		}

		// Gang tags
		private class GangTagLayer
		{
			public GangTagLayer()
			{

			}

			public GangTagLayer(ELayerType a_LayerType, int a_LayerID, int a_Red, int a_Green, int a_Blue, int a_Alpha, float a_fPosX, float a_fPosY, float a_fScale, string a_strText, int a_Font, bool a_bOutline, bool a_bShadow, int a_SpriteID, float a_fWidth, float a_fHeight, float a_fRotation)
			{
				ID = a_LayerID;
				T = a_LayerType;

				R = a_Red;
				G = a_Green;
				B = a_Blue;
				A = a_Alpha;
				X = a_fPosX;
				Y = a_fPosY;
				S = a_fScale;

				Txt = a_strText;
				Font = a_Font;
				OL = a_bOutline;
				SH = a_bShadow;

				SID = a_SpriteID;
				W = a_fWidth;
				H = a_fHeight;
				ROT = a_fRotation;
			}

			// RECTANGLES AND SPRITES
			public float W { get; set; }
			public float H { get; set; }
			// END RECTANGLES

			// SPRITES
			public int SID { get; set; }
			public float ROT { get; set; }
			// END SPRITES

			// TEXT
			public string Txt { get; set; }
			public int Font { get; set; }
			public bool OL { get; set; }
			public bool SH { get; set; }
			// END TEXT

			public ELayerType T { get; set; }
			public int ID { get; set; }

			public int R { get; set; }
			public int G { get; set; }
			public int B { get; set; }
			public int A { get; set; }
			public float X { get; set; }
			public float Y { get; set; }
			public float S { get; set; }

			public override bool Equals(object obj)
			{
				// If the passed object is null
				if (obj == null)
				{
					return false;
				}
				if (!(obj is GangTagLayer))
				{
					return false;
				}
				return (this.Txt == ((GangTagLayer)obj).Txt)
					&& (this.Font == ((GangTagLayer)obj).Font)
					&& (this.OL == ((GangTagLayer)obj).OL)
					&& (this.SH == ((GangTagLayer)obj).SH)
					&& (this.T == ((GangTagLayer)obj).T)
					&& (this.ID == ((GangTagLayer)obj).ID)
					&& (this.R == ((GangTagLayer)obj).R)
					&& (this.G == ((GangTagLayer)obj).G)
					&& (this.B == ((GangTagLayer)obj).B)
					&& (this.A == ((GangTagLayer)obj).A)
					&& (this.X == ((GangTagLayer)obj).X)
					&& (this.Y == ((GangTagLayer)obj).Y)
					&& (this.S == ((GangTagLayer)obj).S);
			}

			public static bool operator ==(GangTagLayer obj1, GangTagLayer obj2)
			{
				return (obj1.Txt == obj2.Txt)
					&& (obj1.Font == obj2.Font)
					&& (obj1.OL == obj2.OL)
					&& (obj1.SH == obj2.SH)
					&& (obj1.T == obj2.T)
					&& (obj1.ID == obj2.ID)
					&& (obj1.R == obj2.R)
					&& (obj1.G == obj2.G)
					&& (obj1.B == obj2.B)
					&& (obj1.A == obj2.A)
					&& (obj1.X == obj2.X)
					&& (obj1.Y == obj2.Y)
					&& (obj1.S == obj2.S);
			}

			public static bool operator !=(GangTagLayer obj1, GangTagLayer obj2)
			{
				return (obj1.Txt != obj2.Txt)
					|| (obj1.Font != obj2.Font)
					|| (obj1.OL != obj2.OL)
					|| (obj1.SH != obj2.SH)
					|| (obj1.T != obj2.T)
					|| (obj1.ID != obj2.ID)
					|| (obj1.R != obj2.R)
					|| (obj1.G != obj2.G)
					|| (obj1.B != obj2.B)
					|| (obj1.A != obj2.A)
					|| (obj1.X != obj2.X)
					|| (obj1.Y != obj2.Y)
					|| (obj1.S != obj2.S);
			}

			public override int GetHashCode()
			{
				return Txt.GetHashCode(StringComparison.Ordinal) ^ Font.GetHashCode() ^ OL.GetHashCode() ^ SH.GetHashCode() ^ T.GetHashCode()
					 ^ ID.GetHashCode() ^ R.GetHashCode() ^ G.GetHashCode() ^ B.GetHashCode() ^ A.GetHashCode()
					  ^ X.GetHashCode() ^ Y.GetHashCode() ^ S.GetHashCode();
			}
		}

		public enum ELayerType
		{
			Text,
			Sprite,
			Rectangle
		}

		private static List<int> InitTestList()
		{
			List<int> lst = new List<int>();
			lst.Add(1);
			lst.Add(2);
			lst.Add(3);
			return lst;
		}

		private string m_strKnownSerializedListData = "AwAAAAEAAAACAAAAAwAAAA==";
		[TestMethod]
		public void TestSerializeKnownList()
		{
			List<int> lstTest = InitTestList();

			BinaryBuffer buffer = BinarySerializer.FastSerialize(lstTest);

			Assert.AreEqual(m_strKnownSerializedListData, Convert.ToBase64String(buffer.GetBytes()));
		}

		[TestMethod]
		public void TestDeserializeKnownList()
		{
			List<int> lstTest = InitTestList();
			List<int> lstComp = BinarySerializer.FastDeserialize<List<int>>(Convert.FromBase64String(m_strKnownSerializedListData));
			Assert.IsTrue(lstComp.SequenceEqual(lstTest));
		}

		[TestMethod]
		public void TestJSONMatchesBinary()
		{
			TestClass classInst = Init();

			BinaryBuffer buffer = BinarySerializer.FastSerialize(classInst);
			TestClass deserialisedBinaryObj = BinarySerializer.FastDeserialize<TestClass>(buffer.GetBytes());

			string strJSON = Newtonsoft.Json.JsonConvert.SerializeObject(classInst);
			TestClass deserializedJSONObj = Newtonsoft.Json.JsonConvert.DeserializeObject<TestClass>(strJSON);

			Assert.AreEqual(classInst, deserialisedBinaryObj);
			Assert.AreEqual(classInst, deserializedJSONObj);
			Assert.AreEqual(deserialisedBinaryObj, deserializedJSONObj);
		}

		// TYPES
		private class SmallTestClass
		{
			public SmallTestClass()
			{

			}

			public string SmallField { get; set; }
			public SmallTestClass2 SubClass { get; set; }

			public override bool Equals(object obj)
			{
				// If the passed object is null
				if (obj == null)
				{
					return false;
				}
				if (!(obj is SmallTestClass))
				{
					return false;
				}
				return (this.SmallField == ((SmallTestClass)obj).SmallField);
			}

			public static bool operator ==(SmallTestClass obj1, SmallTestClass obj2)
			{
				return (obj1.SmallField == obj2.SmallField);
			}

			public static bool operator !=(SmallTestClass obj1, SmallTestClass obj2)
			{
				return (obj1.SmallField != obj2.SmallField);
			}

			public override int GetHashCode()
			{
				return SmallField.GetHashCode(StringComparison.Ordinal);
			}
		}

		private class SmallTestClass2
		{
			public SmallTestClass2()
			{

			}

			public string SmallField { get; set; }

			public override bool Equals(object obj)
			{
				// If the passed object is null
				if (obj == null)
				{
					return false;
				}
				if (!(obj is SmallTestClass2))
				{
					return false;
				}
				return (this.SmallField == ((SmallTestClass2)obj).SmallField);
			}

			public static bool operator ==(SmallTestClass2 obj1, SmallTestClass2 obj2)
			{
				return (obj1.SmallField == obj2.SmallField);
			}

			public static bool operator !=(SmallTestClass2 obj1, SmallTestClass2 obj2)
			{
				return (obj1.SmallField != obj2.SmallField);
			}

			public override int GetHashCode()
			{
				return SmallField.GetHashCode(StringComparison.Ordinal);
			}
		}

		private enum TestEnumType
		{
			Zero = 0,
			One = 1,
			Two = 2
		}


		private class TestClass
		{
			public string Field1 { get; set; }
			public string Field2 { get; set; }
			public string Field3 { get; set; }
			public string Field4 { get; set; }
			public string Field5 { get; set; }

			public int Flags1 { get; set; }
			public int Flags2 { get; set; }
			public int Flags3 { get; set; }
			public int Flags4 { get; set; }
			public int Flags5 { get; set; }
#pragma warning disable CA2227 // Collection properties should be read only
			public List<int> List1 { get; set; }
			public List<string> List2 { get; set; }
			public List<SmallTestClass2> List3 { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only
			public SmallTestClass SmallTestClassInst { get; set; }
			public SmallTestClass SmallTestClassInst2 { get; set; }
			public bool TestBool { get; set; }
			public float TestFloat { get; set; }
			public TestEnumType TestEnum { get; set; }

			public override bool Equals(object obj)
			{
				// If the passed object is null
				if (obj == null)
				{
					return false;
				}
				if (!(obj is TestClass))
				{
					return false;
				}
				return (this.Field1 == ((TestClass)obj).Field1)
					&& (this.Field2 == ((TestClass)obj).Field2)
					&& (this.Field3 == ((TestClass)obj).Field3)
					&& (this.Field4 == ((TestClass)obj).Field4)
					&& (this.Field5 == ((TestClass)obj).Field5)
					&& (this.Flags1 == ((TestClass)obj).Flags1)
					&& (this.Flags2 == ((TestClass)obj).Flags2)
					&& (this.Flags3 == ((TestClass)obj).Flags3)
					&& (this.Flags4 == ((TestClass)obj).Flags4)
					&& (this.Flags5 == ((TestClass)obj).Flags5)
					&& (this.List1.SequenceEqual(((TestClass)obj).List1))
					&& (this.List2.SequenceEqual(((TestClass)obj).List2))
					&& (this.TestEnum == ((TestClass)obj).TestEnum);
			}

			public static bool operator ==(TestClass obj1, TestClass obj2)
			{
				return (obj1.Field1 == obj2.Field1)
					&& (obj1.Field2 == obj2.Field2)
					&& (obj1.Field3 == obj2.Field3)
					&& (obj1.Field4 == obj2.Field4)
					&& (obj1.Field5 == obj2.Field5)
					&& (obj1.Flags1 == obj2.Flags1)
					&& (obj1.Flags2 == obj2.Flags2)
					&& (obj1.Flags3 == obj2.Flags3)
					&& (obj1.Flags4 == obj2.Flags4)
					&& (obj1.Flags5 == obj2.Flags5)
					&& (obj1.List1.SequenceEqual(obj2.List1))
					&& (obj1.List2.SequenceEqual(obj2.List2))
					&& (obj1.TestEnum == obj2.TestEnum);
			}

			public static bool operator !=(TestClass obj1, TestClass obj2)
			{
				return (obj1.Field1 != obj2.Field1)
					|| (obj1.Field2 != obj2.Field2)
					|| (obj1.Field3 != obj2.Field3)
					|| (obj1.Field4 != obj2.Field4)
					|| (obj1.Field5 != obj2.Field5)
					|| (obj1.Flags1 != obj2.Flags1)
					|| (obj1.Flags2 != obj2.Flags2)
					|| (obj1.Flags3 != obj2.Flags3)
					|| (obj1.Flags4 != obj2.Flags4)
					|| (obj1.Flags5 != obj2.Flags5)
					|| !(obj1.List1.SequenceEqual(obj2.List1))
					|| !(obj1.List2.SequenceEqual(obj2.List2))
					|| (obj1.TestEnum != obj2.TestEnum);
			}

			public override int GetHashCode()
			{
				return Field1.GetHashCode(StringComparison.Ordinal) ^ Field2.GetHashCode(StringComparison.Ordinal) ^ Field3.GetHashCode(StringComparison.Ordinal) ^ Field4.GetHashCode(StringComparison.Ordinal) ^ Field5.GetHashCode(StringComparison.Ordinal)
					 ^ Flags1.GetHashCode() ^ Flags2.GetHashCode() ^ Flags3.GetHashCode() ^ Flags4.GetHashCode() ^ Flags5.GetHashCode()
					  ^ List1.GetHashCode() ^ List2.GetHashCode() ^ TestEnum.GetHashCode();
			}

		}
	}
}
#pragma warning restore CA1707 // Identifiers should not contain underscores