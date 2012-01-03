using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Geo
{
    public class RtsdMapIndex
    {
        public class TupleList<T1, T2> : List<Tuple<T1, T2>>
        {
            public void Add(T1 item, T2 item2)
            {
                Add(new Tuple<T1, T2>(item, item2));
            }
        }


        /* 5541-I   102 45 E - 103 00 E / 16 30 N - 16 15 N
         * 5541-II  102 45 E - 103 00 E / 16 15 N - 16 00 N
         * 5541-III 102 30 E - 102 45 E / 16 15 N - 16 00 N
         * 5541-IV  102 30 E - 102 45 E / 16 30 N - 16 15 N
         * 
         * 5441-I   102 15 E - 102 30 E / 16 30 N - 16 15 N
         * 5542-I   102 45 E - 103 00 E / 17 00 N - 16 45 N
         * 
         * starting 4833/4933 to south longitude offset different
         * 
         * northwest corner: 4449
         * northeast corner: 6149
         * 
         * http://www.rtsd.mi.th/service/download/vectorsevice.pdf
         */

        private static List<Int32> ValidFourSquares = new List<Int32>() 
        {
                                    4849, 4949, 5049, 5149,
                  4548, 4648, 4748, 4848, 4948, 5048, 5148, 5248,
                  4547, 4647, 4747, 4847, 4947, 5047, 5147, 5247,
            4446, 4546, 4646, 4746, 4846, 4946, 5046, 5146, 5246,
            4445, 4545, 4645, 4745, 4845, 4945, 5045, 5145, 5245, 5345, 5445, 5545, 5645, 5745, 5845,
                  4544, 4644, 4744, 4844, 4944, 5044, 5144, 5244, 5344, 5444, 5544, 5644, 5744, 5844, 5944,
                        4643, 4743, 4843, 4943, 5043, 5143, 5243, 5343, 5443, 5543, 5643, 5743, 5843, 5943,
                        4642, 4742, 4842, 4942, 5042, 5142, 5242, 5342, 5442, 5542, 5642, 5742, 5842, 5942,
                              4741, 4841, 4941, 5041, 5141, 5241, 5341, 5441, 5541, 5641, 5741, 5841, 5941, 6041,
                              4740, 4840, 4940, 5040, 5140, 5240, 5340, 5440, 5540, 5640, 5740, 5840, 5940, 6040, 6140, 
                        4639, 4739, 4839, 4939, 5039, 5139, 5239, 5339, 5439, 5539, 5639, 5739, 5839, 5939, 6039, 6139,
                        4638, 4738, 4838, 4938, 5038, 5138, 5238, 5338, 5438, 5538, 5638, 5738, 5838, 5938, 6038, 6138,
                              4737, 4837, 4937, 5037, 5137, 5237, 5337, 5437, 5537, 5637, 5737, 5837, 5937, 6037,
                                    4836, 4936, 5036, 5136, 5236, 5336, 5436, 5536,
                                    4835, 4935, 5035, 5135, 5235, 5335, 5435, 5535,
                                    4834, 4934, 5034, 5134, 5234, 5334, 5434, 5534,
                                    4833, 4933,                         5433, 5533,
                                    4832, 4932,                         5432, 5532,
                                    4831, 4931,
                              4730, 4830,
                              4729, 4829,
                        4628, 4728, 4828, 4928,
                        4627, 4727, 4827, 4927,
                        4626, 4726, 4826, 4926, 5026,
                        4625, 4725, 4825, 4925, 5025,
                        4624, 4724, 4824, 4924, 5024,
                                    4823, 4923, 5023, 5123,
                                    4822, 4922, 5022, 5122, 5222, 5322,
                                                5021, 5121, 5221, 5321, 5421,
                                                            5220, 5320, 
        };

        private static TupleList<Int32, Int32> InvalidSheets = new TupleList<Int32, Int32>()
        { 
            {4849,4},
            {5149,1}, {5149,2}, {5149,4},

            {4748,4},
            {4648,1},
            {4548,1}, {4548,3}, {4548,4},
            {5148,1},
            {5248,1}, {5248,4},

            {4547,3}, {4547,4},

            {4446,1}, {4446,3}, {4446,4},

            {4445,2}, {4445,3}, {4445,4},
            {5245,1}, {5245,2},
            {5345,1}, {5345,3}, {5345,4},
            {5445,1}, {5445,4},
            {5545,1}, {5545,3}, {5545,4},
            {5845,1}, {5845,2},

            {4544,3},
            {5944,1}, {5944,2}, {5944,4},

            {4643,3},  

            {4642,2}, {4642,3}, {4642,4},

            {6041,1},

            {6140,1}, {6140,2},

            {4639,4},
            {6139,1}, {6139,2},

            {4638,3},
            {6138,1}, {6138,2},

            {4737,3},
            {5637,2},
            {5737,2}, {5737,3},
            {5837,2}, {5837,3},
            {5937,2}, {5937,3},
            {6037,2},

            {5536,2},

            {5035,2},
            {5135,3}, {5135,4},
            {5535,1}, {5535,2}, {5535,4},

            {4834,3},
            {5034,1}, {5034,2}, {5034,3},
            {5134,3},
            {5534,1}, {5534,2},

            {4833,3}, {4833,4},
            {5433,3},

            {4832,1}, {4832,3}, {4832,4},
            {4932,2},
            {5432,2}, {5432,3}, {5432,4},

            {4831,4},
            {4931,1}, {4931,2}, {4931,3},

            {4730,3}, {4730,4},

            {4829,1}, {4829,2},

            {4628,1}, {4628,3}, {4628,4},
            {4828,1}, {4828,2},
            {4928,1}, {4928,4},

            {4627,3}, {4627,4},

            {4626,4},
            {5026,1}, {5026,2}, {5026,4},

            {4625,3}, {4625,4},
            {5025,1},

            {4624,2}, {4624,3}, {4624,4},
            {4724,3},

            {4823,3},
            {5123,1}, {5123,2}, {5123,4},

            {4822,1}, {4822,3}, {4822,4},
            {5322,1}, {5322,2},

            {5021,1}, {5021,2}, {5021,3},
            {5121,2}, {5121,3},
            {5421,1}, {5421,2},

            {5220,2},
            {5320,2}, {5320,3},
        };

        private static TupleList<Int32, Int32> SheetsLongitude18Minutes = new TupleList<Int32, Int32>()
        {
            {4827,4},
            {4927,4},
            {4927,1},
        };
        private static TupleList<Int32, Int32> SheetsLatitude185Minutes = new TupleList<Int32, Int32>()
        {
            {4928,2},
            {4928,3},
        };
        private static TupleList<Int32, Int32> Sheets3MinutesEast = new TupleList<Int32, Int32>()
        {
            {4627,1},
            {4727,4},
            {4727,1},
            {4827,1},
        };

        // special cases: 4822 II - different offset in latitude and longitude
        // special cases: 4827 IV, 4927 IV, 4927 I - 3 minute more wide in longitude
        // 4627 I, 4727 IV, 4727 I, 4827 IV - starts 3 minute more east; 4827 IV 4 minute more wide
        // 4928 II 6 minute more east
        // 4928 II, 4928 III 18.5 minute more high

        public static String IndexL7018(GeoPoint location)
        {
            if ( location == null )
            {
                throw new ArgumentNullException("location");
            }
            if ( !MapIndexL7018.Any() )
            {
                CalcIndexList();
            }

            var entry = MapIndexL7018.Find(x => x.IsInside(location));

            if ( entry == null )
            {
                throw new ArgumentOutOfRangeException("location");
            }
            else
            {
                return entry.Name;
            }
        }

        public static List<RtdsMapFrame> MapIndexL7018 = new List<RtdsMapFrame>();
        private static void Add7018Entry(GeoPoint corner, Int32 number, Byte subindex)
        {
            if ( !InvalidSheets.Any(x => (x.Item1 == number) && (x.Item2 == subindex)) )
            {
                Double standardExtend = 15.0 / 60.0;
                Double latitudeExtend = standardExtend;
                Double longitudeExtend = standardExtend;

                GeoPoint actualCorner = new GeoPoint(corner);
                String subIndexName = String.Empty;
                switch ( subindex )
                {
                    case 1:
                        subIndexName = "I";
                        actualCorner.Longitude += standardExtend;
                        break;
                    case 2:
                        subIndexName = "II";
                        actualCorner.Longitude += standardExtend;
                        actualCorner.Latitude -= standardExtend;
                        break;
                    case 3:
                        subIndexName = "III";
                        actualCorner.Latitude -= standardExtend;
                        break;
                    case 4:
                        subIndexName = "IV";
                        break;
                }

                if ( SheetsLongitude18Minutes.Any(x => (x.Item1 == number) && (x.Item2 == subindex)) )
                {
                    longitudeExtend = 18.0 / 60.0;
                }
                if ( SheetsLatitude185Minutes.Any(x => (x.Item1 == number) && (x.Item2 == subindex)) )
                {
                    latitudeExtend = 18.5 / 60.0;
                }
                if ( Sheets3MinutesEast.Any(x => (x.Item1 == number) && (x.Item2 == subindex)) )
                {
                    actualCorner.Longitude += 3.0 / 60.0;
                }
                // add more special cases

                MapIndexL7018.Add(new RtdsMapFrame(actualCorner, latitudeExtend, longitudeExtend, string.Format("{0:####} {1}", number, subIndexName)));
            }
        }
        public static void CalcIndexList()
        {
            if ( MapIndexL7018.Any() )
            {
                return;
            }
            foreach ( var group in ValidFourSquares )
            {
                Int32 latitudeIndex = group % 100;
                Int32 longitudeIndex = group / 100;
                Double latitude = (latitudeIndex - 42 + 34) / 2.0;
                Double longitude = (longitudeIndex - 55 + 205) / 2.0;

                GeoPoint northWestCorner = new GeoPoint(latitude, longitude);
                for ( Byte i = 1 ; i <= 4 ; i++ )
                {
                    Add7018Entry(northWestCorner, group, i);
                }
            }
        }
    }
}
