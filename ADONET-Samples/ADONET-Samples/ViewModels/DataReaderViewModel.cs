﻿using ADONET_Samples.ManagerClasses;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace ADONET_Samples.ViewModels
{
    public class DataReaderViewModel : ViewModelBase
    {
        #region Private Variables
        private ObservableCollection<Product> _Products = new ObservableCollection<Product>();
        private ObservableCollection<ProductCategory> _Categories = new ObservableCollection<ProductCategory>();
        #endregion

        #region Public Properties
        /// <summary>
        /// Get/Set Products collection
        /// </summary>
        public ObservableCollection<Product> Products
        {
            get { return _Products; }
            set
            {
                _Products = value;
                RaisePropertyChanged("Products");
            }
        }

        /// <summary>
        /// Get/Set Category collection
        /// </summary>
        public ObservableCollection<ProductCategory> Categories
        {
            get { return _Categories; }
            set
            {
                _Categories = value;
                RaisePropertyChanged("Categories");
            }
        }
        #endregion

        #region GetProductsAsDataReader Method
        public void GetProductsAsDataReader()
        {
            StringBuilder sb = new StringBuilder(1024);

            // Initialize Data Reader to null in case of an error
            SqlDataReader dr = null;

            // Create a connection
            using (SqlConnection cnn = new SqlConnection(AppSettings.ConnectionString))
            {
                // Open the connection
                cnn.Open();

                // Create command object
                using (SqlCommand cmd = new SqlCommand(ProductManager.PRODUCT_SQL, cnn))
                {
                    cmd.CommandType = CommandType.Text;
                    using (dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (dr.Read())
                        {
                            sb.AppendLine("Product: " + dr["ID"].ToString());
                            sb.AppendLine(dr["ProductName"].ToString());
                            sb.AppendLine(Convert.ToDateTime(dr["IntroductionDate"]).ToShortDateString());
                            sb.AppendLine(Convert.ToDecimal(dr["Price"]).ToString("c"));
                            sb.AppendLine();
                        }
                    }
                }
            }

            ResultText = sb.ToString();
        }
        #endregion

        #region GetProductsAsGenericList Method
        public ObservableCollection<Product> GetProductsAsGenericList()
        {
            Products.Clear();

            // Initialize DataReader to null in case of an error
            SqlDataReader dr = null;

            // Create a connection
            using (SqlConnection cnn = new SqlConnection(AppSettings.ConnectionString))
            {
                // Open the connection
                cnn.Open();

                // Create command object
                using (SqlCommand cmd = new SqlCommand(ProductManager.PRODUCT_SQL, cnn))
                {
                    dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (dr.Read())
                    {
                        Products.Add(new Product
                        {
                            // Use Convert class
                            ProductId = Convert.ToInt32(dr["ID"]),
                            // Use GetString() and GetOrdinal()
                            
                            ProductName = dr.GetString(dr.GetOrdinal("ProductName")),
                            // Use GetDateTime() and GetOrdinal()
                            //IntroductionDate = dr.GetDateTime(dr.GetOrdinal("IntroductionDate")),
                            IntroductionDate = DateTime.Parse(dr.GetString(dr.GetOrdinal("IntroductionDate"))),
                            Url = dr["Url"].ToString(),
                            Price = Convert.ToDecimal(dr["Price"]),
                            // Check for Null
                            //RetireDate = dr.IsDBNull(dr.GetOrdinal("RetireDate")) ? (DateTime?)null : Convert.ToDateTime(dr["RetireDate"]),
                            //ProductCategoryId = dr.IsDBNull(dr.GetOrdinal("ProductCategoryId")) ? (int?)null : Convert.ToInt32(dr["ProductCategoryId"]),
                            RetireDate = DateTime.Now,
                            ProductCategoryId = Convert.ToInt32("1")
                        });
                    }
                }
            }

            return Products;
        }
        #endregion

        #region GetProductsUsingFieldValue Method
        public ObservableCollection<Product> GetProductsUsingFieldValue()
        {
            Products.Clear();

            // Initialize DataReader to null in case of an error
            SqlDataReader dataReader = null;

            // Create a connection
            using (SqlConnection cnn = new SqlConnection(AppSettings.ConnectionString))
            {
                // Open the connection
                cnn.Open();

                // Create command object
                using (SqlCommand cmd = new SqlCommand(ProductManager.PRODUCT_SQL, cnn))
                {
                    dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (dataReader.Read())
                    {
                        Products.Add(new Product
                        {
                            ProductId = dataReader.GetFieldValue<int>(dataReader.GetOrdinal("ID")),
                            ProductName = dataReader.GetFieldValue<string>(dataReader.GetOrdinal("ProductName")),
                            //IntroductionDate = dataReader.GetFieldValue<DateTime>(dataReader.GetOrdinal("IntroductionDate")),
                            IntroductionDate = DateTime.Now,
                            Url = dataReader.GetFieldValue<string>(dataReader.GetOrdinal("Url")),
                            Price = dataReader.GetFieldValue<decimal>(dataReader.GetOrdinal("Price")),
                            // NOTE: GetFieldValue() does not work on nullable fields
                            //RetireDate = dr.GetFieldValue<DateTime?>(dr.GetOrdinal("RetireDate")),
                            //ProductCategoryId = dr.GetFieldValue<int?>(dr.GetOrdinal("ProductCategoryId"))
                            RetireDate = dataReader.IsDBNull(dataReader.GetOrdinal("RetireDate")) ? (DateTime?)null : Convert.ToDateTime(dataReader["RetireDate"]),
                            ProductCategoryId = dataReader.IsDBNull(dataReader.GetOrdinal("ProductCategoryId")) ? (int?)null : Convert.ToInt32(dataReader["ProductCategoryId"])
                        });
                    }
                }
            }

            return Products;
        }
        #endregion

        #region GetProductsUsingExtensionMethods Method
        public ObservableCollection<Product> GetProductsUsingExtensionMethods()
        {
            Products.Clear();

            // Initialize DataReader to null in case of an error
            SqlDataReader dr = null;

            // Create a connection
            using (SqlConnection cnn = new SqlConnection(AppSettings.ConnectionString))
            {
                // Open the connection
                cnn.Open();

                // Create command object
                using (SqlCommand cmd = new SqlCommand(ProductManager.PRODUCT_SQL, cnn))
                {
                    dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (dr.Read())
                    {
                        Products.Add(new Product
                        {
                            ProductId = dr.GetFieldValue<int>("ID"),
                            ProductName = dr.GetFieldValue<string>("ProductName"),
                            //IntroductionDate = dr.GetFieldValue<DateTime>("IntroductionDate"),
                            IntroductionDate = DateTime.Now,
                            Url = dr.GetFieldValue<string>("Url"),
                            Price = dr.GetFieldValue<decimal>("Price"),
                            RetireDate = dr.GetFieldValue<DateTime?>("RetireDate"),
                            ProductCategoryId = dr.GetFieldValue<int?>("ProductCategoryId")
                        });
                    }
                }
            }

            return Products;
        }
        #endregion

        #region GetMultipleResultSets Method
        public void GetMultipleResultSets()
        {
            // Initialize DataReader to null in case of an error
            SqlDataReader dr = null;
            Products.Clear();
            Categories.Clear();

            // Create SQL statement to submit
            string sql = ProductManager.PRODUCT_SQL;
            sql += ";" + ProductManager.PRODUCT_CATEGORY_SQL;

            // Create a connection
            using (SqlConnection cnn = new SqlConnection(AppSettings.ConnectionString))
            {
                // Open the connection
                cnn.Open();

                // Create command object
                using (SqlCommand cmd = new SqlCommand(sql, cnn))
                {
                    dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (dr.Read())
                    {
                        Products.Add(new Product
                        {
                            ProductId = dr.GetFieldValue<int>("ID"),
                            ProductName = dr.GetFieldValue<string>("ProductName"),
                            //IntroductionDate = dr.GetFieldValue<DateTime>("IntroductionDate"),
                            IntroductionDate = DateTime.Now,
                            Url = dr.GetFieldValue<string>("Url"),
                            Price = dr.GetFieldValue<decimal>("Price"),
                            RetireDate = dr.GetFieldValue<DateTime?>("RetireDate"),
                            ProductCategoryId = dr.GetFieldValue<int?>("ProductCategoryId")
                        });
                    }

                    // Move to next result set
                    dr.NextResult();
                    while (dr.Read())
                    {
                        Categories.Add(new ProductCategory
                        {
                            ProductCategoryId = dr.GetFieldValue<int>("ID"),
                            CategoryName = dr.GetFieldValue<string>("CategoryName")
                        });
                    }
                }
            }
        }
        #endregion
    }
}
