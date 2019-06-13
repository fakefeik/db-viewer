﻿using System;
using System.Threading.Tasks;
using AutoFixture;
using Cassandra;
using FluentAssertions;
using GroBuf;
using GroBuf.DataMembersExtracters;
using Kontur.DBViewer.Core.DTO;
using Kontur.DBViewer.Core.DTO.TypeInfo;
using Kontur.DBViewer.Core.Schemas;
using Kontur.DBViewer.Recipes.CQL.DTO;
using Kontur.DBViewer.SampleApi;
using Kontur.DBViewer.SampleApi.Controllers;
using Kontur.DBViewer.SampleApi.Impl;
using Kontur.DBViewer.SampleApi.Impl.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Kontur.DBViewer.Tests.ApiTests
{
    [TestFixture]
    public class ApiTests
    {
        private WebApiService service;
        private ApiClient client;
        private ClassTypeInfo testClassShape;
        private ISerializer serializer;
        private Fixture fixture;

        [OneTimeSetUp]
        public void SetUp()
        {
            service = new WebApiService();
            client = new ApiClient();
            serializer = new Serializer(new AllPropertiesExtractor());
            fixture = new Fixture();
            var schemaRegistry = new SchemaRegistry();
            schemaRegistry.Add(
                new Schema
                {
                    Description = new SchemaDescription
                    {
                        Countable = true,
                        SchemaName = "SampleSchema",
                        MaxCountLimit = 10_000,
                        DefaultCountLimit = 100,
                        EnableDefaultSearch = false,
                    },
                    Types = new[]
                    {
                        new TypeDescription
                        {
                            Type = typeof(TestClass),
                            TypeIdentifier = "TestClass",
                        },
                    },
                    PropertyDescriptionBuilder = new SamplePropertyDescriptionBuilder(),
                    ConnectorsFactory = new SampleIdbConnectorFactory(),
                    CustomPropertyConfigurationProvider = new SampleCustomPropertyConfigurationProvider(serializer),
                }
            );
            var localTimeShape = new ClassTypeInfo
            {
                Properties = new []
                {
                    new Property
                    {
                        TypeInfo = new IntTypeInfo(false),
                        Description = new PropertyDescription
                        {
                            Name = "Hour"
                        }
                    },
                    new Property
                    {
                        TypeInfo = new IntTypeInfo(false),
                        Description = new PropertyDescription
                        {
                            Name = "Minute"
                        }
                    },
                    new Property
                    {
                        TypeInfo = new IntTypeInfo(false),
                        Description = new PropertyDescription
                        {
                            Name = "Second"
                        }
                    },
                    new Property
                    {
                        TypeInfo = new IntTypeInfo(false),
                        Description = new PropertyDescription
                        {
                            Name = "Nanoseconds"
                        },
                    }
                }
            };
            var testClassWithAllPrimitivesShape = new ClassTypeInfo
            {
                Properties = new[]
                {
                    new Property
                    {
                        TypeInfo = new StringTypeInfo(),
                        Description = new PropertyDescription
                        {
                            Name = "String",
                        }
                    },
                    new Property
                    {
                        TypeInfo = new StringTypeInfo(),
                        Description = new PropertyDescription
                        {
                            Name = "Guid",
                        }
                    },
                    new Property
                    {
                        TypeInfo = new StringTypeInfo(),
                        Description = new PropertyDescription
                        {
                            Name = "NullableGuid",
                        }
                    },
                    new Property
                    {
                        TypeInfo = new ByteTypeInfo(false),
                        Description = new PropertyDescription
                        {
                            Name = "Byte",
                        }
                    },
                    new Property
                    {
                        TypeInfo = new ByteTypeInfo(true),
                        Description = new PropertyDescription
                        {
                            Name = "NullableByte",
                        }
                    },
                    new Property
                    {
                        TypeInfo = new CharTypeInfo(false),
                        Description = new PropertyDescription
                        {
                            Name = "Char",
                        }
                    },
                    new Property
                    {
                        TypeInfo = new CharTypeInfo(true),
                        Description = new PropertyDescription
                        {
                            Name = "NullableChar",
                        }
                    },
                    new Property
                    {
                        TypeInfo = new IntTypeInfo(false),
                        Description = new PropertyDescription
                        {
                            Name = "Int",
                        }
                    },
                    new Property
                    {
                        TypeInfo = new IntTypeInfo(true),
                        Description = new PropertyDescription
                        {
                            Name = "NullableInt",
                        }
                    },
                    new Property
                    {
                        TypeInfo = new LongTypeInfo(false),
                        Description = new PropertyDescription
                        {
                            Name = "Long",
                        }
                    },
                    new Property
                    {
                        TypeInfo = new LongTypeInfo(true),
                        Description = new PropertyDescription
                        {
                            Name = "NullableLong",
                        }
                    },
                    new Property
                    {
                        TypeInfo = new DecimalTypeInfo(false),
                        Description = new PropertyDescription
                        {
                            Name = "Decimal",
                        }
                    },
                    new Property
                    {
                        TypeInfo = new DecimalTypeInfo(true),
                        Description = new PropertyDescription
                        {
                            Name = "NullableDecimal",
                        }
                    },
                    new Property
                    {
                        TypeInfo = new DateTimeTypeInfo(false),
                        Description = new PropertyDescription
                        {
                            Name = "DateTime",
                        }
                    },
                    new Property
                    {
                        TypeInfo = new DateTimeTypeInfo(true),
                        Description = new PropertyDescription
                        {
                            Name = "NullableDateTime",
                        }
                    },
                    new Property
                    {
                        TypeInfo = new DateTimeTypeInfo(false),
                        Description = new PropertyDescription
                        {
                            Name = "DateTimeOffset",
                        }
                    },
                    new Property
                    {
                        TypeInfo = new DateTimeTypeInfo(true),
                        Description = new PropertyDescription
                        {
                            Name = "NullableDateTimeOffset",
                        }
                    },
                    new Property
                    {
                        TypeInfo = new EnumTypeInfo(false, new[]{"FirstValue", "SecondValue"}),
                        Description = new PropertyDescription
                        {
                            Name = "Enum",
                        }
                    },
                    new Property
                    {
                        TypeInfo = new EnumTypeInfo(true, new[]{"FirstValue", "SecondValue"}),
                        Description = new PropertyDescription
                        {
                            Name = "NullableEnum",
                        }
                    },
                    new Property
                    {
                        TypeInfo = new EnumerableTypeInfo(new IntTypeInfo(false)),
                        Description = new PropertyDescription
                        {
                            Name = "Array",
                        },
                    },
                    new Property
                    {
                        TypeInfo = new EnumerableTypeInfo(new IntTypeInfo(false)),
                        Description = new PropertyDescription
                        {
                            Name = "List",
                        },
                    },
                    new Property
                    {
                        TypeInfo = new DictionaryTypeInfo(new StringTypeInfo(), new IntTypeInfo(false)),
                        Description = new PropertyDescription
                        {
                            Name = "Dictionary",
                        },
                    },
                    new Property
                    {
                        TypeInfo = new HashSetTypeInfo(new StringTypeInfo()),
                        Description = new PropertyDescription
                        {
                            Name = "HashSet",
                        },
                    },
                }
            };
            testClassShape =
                new ClassTypeInfo
                {
                    Properties = new[]
                    {
                        new Property
                        {
                            TypeInfo = new StringTypeInfo(),
                            Description = new PropertyDescription
                            {
                                Name = "Id",
                                IsIdentity = true,
                                IsSearchable = true,
                                AvailableFilters = new[] {FilterType.No, FilterType.Equals, FilterType.NotEquals},
                            },
                        },
                        new Property
                        {
                            TypeInfo = testClassWithAllPrimitivesShape,
                            Description = new PropertyDescription
                            {
                                Name = "Content",
                            },
                        },
                        new Property
                        {
                            TypeInfo = new ClassTypeInfo
                            {
                                Properties = new[]
                                {
                                    new Property
                                    {
                                        TypeInfo = testClassWithAllPrimitivesShape,
                                        Description = new PropertyDescription
                                        {
                                            Name = "Content",
                                        }
                                    }
                                }
                            },
                            Description = new PropertyDescription
                            {
                                Name = "Serialized",
                            },
                        },
                        new Property
                        {
                            TypeInfo = localTimeShape,
                            Description = new PropertyDescription
                            {
                                Name = "LocalTime",
                            }
                        },
                    },
                };
            SchemaRegistryProvider.SetSchemaRegistry(schemaRegistry);
            service.Start(7777);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            try
            {
                service.Stop();
            }
            catch
            {
                // ignored
            }
        }

        [Test]
        public async Task Test_Read()
        {
            var customPropertyContent = fixture.Create<ClassForSerialization>();
            var @object = fixture.Build<TestClass>()
                .With(x => x.Serialized, serializer.Serialize(customPropertyContent))
                .Create();
            FillDataBase(@object);
            var result = await client.Read("TestClass", new[]
            {
                new Filter
                {
                    Type = FilterType.Equals,
                    Field = "Id",
                    Value = @object.Id,
                }
            });
            Console.WriteLine($"Object Id: {@object.Id}");

            CheckShape(result.TypeInfo, testClassShape);
            CheckObject(result.Object, new ExpandedTestClass
            {
                Id = @object.Id,
                Content = @object.Content,
                Serialized = customPropertyContent,
                LocalTime = new CassandraLocalTime
                {
                    Hour = @object.LocalTime.Hour,
                    Minute = @object.LocalTime.Minute,
                    Second = @object.LocalTime.Second,
                    Nanoseconds = @object.LocalTime.Nanoseconds,
                }
            });
        }

        [Test]
        public async Task Test_Write()
        {
            var oldCustomPropertyContent = fixture.Create<ClassForSerialization>();
            var oldObject = fixture.Build<TestClass>()
                .With(x => x.Serialized, serializer.Serialize(oldCustomPropertyContent))
                .Create();
            var newCustomPropertyContent = fixture.Create<ClassForSerialization>();
            var newObject = new TestClass
            {
                Id = oldObject.Id,
                Content = fixture.Create<TestClassWithAllPrimitives>(),
                Serialized = serializer.Serialize(newCustomPropertyContent),
                LocalTime = new LocalTime(10, 20, 30, 268)
            };
            var newObjectExpanded = new ExpandedTestClass
            {
                Id = oldObject.Id,
                Content = newObject.Content,
                Serialized = newCustomPropertyContent,
                LocalTime = new CassandraLocalTime
                {
                    Hour = 10,
                    Minute = 20,
                    Second = 30,
                    Nanoseconds = 268
                }
            };

            FillDataBase(oldObject);

            var result = await client.Write("TestClass", newObjectExpanded);
            result.Should().BeEquivalentTo(newObjectExpanded);

            CheckDataBaseContent(newObject);
        }
        
        

        [Test]
        public async Task Test_List()
        {
            var result = await client.GetTypesDescription();
            var schemaDescription = new SchemaDescription
            {
                Countable = true,
                SchemaName = "SampleSchema",
                MaxCountLimit = 10_000,
                DefaultCountLimit = 100,
                EnableDefaultSearch = false,
            };
            result.Should().BeEquivalentTo(new TypesListModel
            {
                Types = new[]
                {
                    new TypeModel
                    {
                        Name = "TestClass",
                        SchemaDescription = schemaDescription,
                        Shape = testClassShape,
                    },
                }
            });
        }

        private void FillDataBase(params TestClass[] objects)
        {
            SampleDataBase.Instance = new SampleDataBase(objects);
        }

        private void CheckDataBaseContent(params TestClass[] objects)
        {
            SampleDataBase.Instance.GetContent().Should().BeEquivalentTo(objects);
        }

        private void CheckObject<T>(object actual, T expected)
        {
            JsonConvert.DeserializeObject<T>(((JObject) actual).ToString()).Should()
                .BeEquivalentTo(expected);
        }

        private void CheckShape(TypeInfo actual, TypeInfo expected)
        {
            actual.Should().BeEquivalentTo(expected, x => x.RespectingRuntimeTypes());
        }
    }
}