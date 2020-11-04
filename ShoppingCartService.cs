namespace Sabio.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        IDataProvider _data = null;
        public ShoppingCartService(IDataProvider data)
        {
            _data = data;
        }
        public int Add(ShoppingCartAddRequest model, int userId)
        {
            int id = 0;

            string procName = "dbo.ShoppingCart_InsertV3";
            _data.ExecuteNonQuery(procName,
               inputParamMapper: delegate (SqlParameterCollection col)
               {
                   AddCommonParams(model, col);

                   SqlParameter idOut = new SqlParameter("@cartId", SqlDbType.Int)
                   {
                       Direction = ParameterDirection.Output
                   };
                   col.Add(idOut);
                   col.AddWithValue("@createdBy", userId);
                   col.AddWithValue("@modifiedBy", userId);

               },
               returnParameters: delegate (SqlParameterCollection returnCollection)
               {
                   object oId = returnCollection["@cartId"].Value;

                   int.TryParse(oId.ToString(), out id);
               });

            return id;
        }
        public int MultiAdd(ShoppingCartMultiAddRequest model, int userId)
        {
            int id = 0;
            DataTable itemParam = MapItemsToTable(model.Items);
            string procName = "dbo.ShoppingCart_MultiInsertV2";
            _data.ExecuteNonQuery(procName,
               inputParamMapper: delegate (SqlParameterCollection col)
               {
                   col.AddWithValue("@CartItems", itemParam);

                   SqlParameter idOut = new SqlParameter("@cartId", SqlDbType.Int)
                   {
                       Direction = ParameterDirection.Output
                   };
                   col.Add(idOut);
                   col.AddWithValue("@createdBy", userId);
                   col.AddWithValue("@modifiedBy", userId);

               },
               returnParameters: delegate (SqlParameterCollection returnCollection)
               {
                   object oId = returnCollection["@cartId"].Value;

                   int.TryParse(oId.ToString(), out id);
               });

            return id;
        }
        public void Update(ShoppingCartUpdateRequest model, int userId)
        {
            string procName = "dbo.ShoppingCart_UpdateV3";
            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    AddCommonParams(model, col);
                    col.AddWithValue("@cartId", model.Id);
                    col.AddWithValue("@modifiedBy", userId);
                },
                returnParameters: null);
        }
        public void Delete(int id)
        {
            string procName = "dbo.ShoppingCart_Delete_ById";

            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@cartId", id);
                },
                returnParameters: null);

        }
        public void DeleteByUser(int userId)
        {
            string procName = "dbo.ShoppingCart_Delete_ByUserId";

            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@createdBy", userId);
                },
                returnParameters: null);

        }
        public Paged<ShoppingCart> GetAll(int pageIndex, int pageSize)
        {
            Paged<ShoppingCart> pagedResult = null;

            List<ShoppingCart> list = null;

            int totalCount = 0;

            _data.ExecuteCmd(
             "dbo.ShoppingCart_SelectAllV3",
             inputParamMapper: delegate (SqlParameterCollection col)
             {
                 col.AddWithValue("@pageIndex", pageIndex);
                 col.AddWithValue("@pageSize", pageSize);
             },
             singleRecordMapper: delegate (IDataReader reader, short set)
             {
                 ShoppingCart cart = MapCart(reader, out int startingIndex);


                 if (totalCount == 0)
                 {
                     totalCount = reader.GetSafeInt32(startingIndex++);
                 }


                 if (list == null)
                 {
                     list = new List<ShoppingCart>();
                 }

                 list.Add(cart);
             }

         );
            if (list != null)
            {
                pagedResult = new Paged<ShoppingCart>(list, pageIndex, pageSize, totalCount);
            }

            return pagedResult;
        }
        public ShoppingCart Get(int id)
        {
            string proc = "dbo.ShoppingCart_Select_ByIdV3";
            ShoppingCart cart = null;

            _data.ExecuteCmd(proc, delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@cartId", id);

            },
            delegate (IDataReader reader, short set)
            {
                cart = MapCart(reader, out int startingIndex);
            }
            );

            return cart;
        }
        public List<ShoppingCart> GetByCreatedBy(int userId)
        {
            List<ShoppingCart> list = null;

            _data.ExecuteCmd(
             "dbo.ShoppingCart_Select_ByCreatedByV3",
             inputParamMapper: delegate (SqlParameterCollection col)
             {
                 col.AddWithValue("@createdBy", userId);
             },
             singleRecordMapper: delegate (IDataReader reader, short set)
             {
                 ShoppingCart cart = MapCart(reader, out int startingIndex);

                 if (list == null)
                 {
                     list = new List<ShoppingCart>();
                 }

                 list.Add(cart);
             }
         );
            return list;
        }
        private void AddCommonParams(ShoppingCartAddRequest model, SqlParameterCollection col)
        {
            col.AddWithValue("@productId", model.ProductId);
            col.AddWithValue("@quantity", model.Quantity);
            col.AddWithValue("@specRequests", model.SpecialRequests);
        }
        private static ShoppingCart MapCart(IDataReader reader, out int startingIndex)
        {
            ShoppingCart cart = new ShoppingCart();

            startingIndex = 0;

            cart.Id = reader.GetSafeInt32(startingIndex++);
            cart.ProductId = reader.GetSafeInt32(startingIndex++);
            cart.ProductName = reader.GetSafeString(startingIndex++);
            cart.Quantity = reader.GetSafeInt32(startingIndex++);
            cart.SpecialRequests = reader.GetSafeString(startingIndex++);
            cart.ItemCost = reader.GetSafeDecimal(startingIndex++);
            cart.VendorName = reader.GetSafeString(startingIndex++);
            cart.VendorId = reader.GetSafeInt32(startingIndex++);
            cart.ProductImages = reader.DeserializeObject<List<Files>>(startingIndex++);
            cart.CreatedBy = reader.GetSafeInt32(startingIndex++);
            cart.ModifiedBy = reader.GetSafeInt32(startingIndex++);
            cart.DateCreated = reader.GetSafeDateTime(startingIndex++);
            cart.DateModified = reader.GetSafeDateTime(startingIndex++);

            return cart;
        }
        private DataTable MapItemsToTable(List<ShoppingCartAddRequest> Items)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ProductId", typeof(Int32));
            dt.Columns.Add("Quantity", typeof(Int32));
            dt.Columns.Add("SpecialRequests", typeof(string) ?? null);
            foreach (ShoppingCartAddRequest item in Items)
            {
                DataRow dr = dt.NewRow();
                int startingIndex = 0;

                dr.SetField(startingIndex++, item.ProductId);
                dr.SetField(startingIndex++, item.Quantity);
                dr.SetField(startingIndex++, item.SpecialRequests);

                dt.Rows.Add(dr);

            }
            return dt;
        }
    }
}
