//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Wodsoft.ComBoost.Data.Entity;
//using Microsoft.Extensions.DependencyInjection;
//using System.ComponentModel;

//namespace Wodsoft.ComBoost.Data
//{
//    public static class EntityPagerExtension
//    {
//        static EntityPagerExtension()
//        {
//            DefaultPageSize = 20;
//            PageSizeOptions = new int[] { 10, 20, 30, 50 };
//        }

//        public static int DefaultPageSize { get; set; }

//        public static int[] PageSizeOptions { get; set; }
//    }

//    public class EntityPagerExtension<T> : IDomainServiceEventHandler<EntityQueryEventArgs<T>>
//        where T : class, IEntity, new()
//    {
//        //        private Task Service_Executed(IDomainExecutionContext context)
//        //        {
//        //            if (context.Result != null && context.Result is IEntityViewModel)
//        //            {
//        //                dynamic model = context.Result;
//        //                model.CurrentPage = context.DomainContext.DataBag.Page;
//        //                model.CurrentSize = context.DomainContext.DataBag.Size;
//        //                model.TotalPage = context.DomainContext.DataBag.TotalPage;
//        //                model.PageSizeOption = EntityPagerExtension.PageSizeOptions;
//        //                model.TotalCount = context.DomainContext.DataBag.TotalCount;
//        //            }
//        //            return Task.CompletedTask;
//        //        }

//        private Task Service_EntityQuery(IDomainExecutionContext context, EntityQueryEventArgs<T> e)
//        {
//            var valueProvider = context.DomainContext.GetRequiredService<IValueProvider>();
//            var option = context.DomainContext.Options.GetOption<EntityPagerOption>();
//            if (option == null)
//            {
//                option = new EntityPagerOption();
//                context.DomainContext.Options.SetOption(option);
//            }
//            if (option.CurrentPage == 0)
//            {
//                int page = valueProvider.GetValue<int>("page");
//                if (page == 0)
//                    page = 1;
//                option.CurrentPage = page;
//            }
//            if (option.CurrentSize == 0)
//            {
//                int size = valueProvider.GetValue<int>("size");
//                if (size == 0)
//                    if (option.DefaultSize == 0)
//                        size = EntityPagerExtension.DefaultPageSize;
//                    else
//                        size = option.DefaultSize;
//                option.CurrentSize = size;
//            }
//            if (option.PageSizeOption == null)
//                option.PageSizeOption = EntityPagerExtension.PageSizeOptions;
//            return Task.CompletedTask;
//            //var databaseContext = context.DomainContext.GetRequiredService<IDatabaseContext>();
//            //var entityContext = databaseContext.GetContext<T>();
//            //var count = await entityContext.CountAsync(e.Queryable);
//            //e.Queryable = e.Queryable.Skip((page - 1) * size).Take(size);
//            //context.DomainContext.DataBag.Page = page;
//            //context.DomainContext.DataBag.Size = size;
//            //context.DomainContext.DataBag.TotalCount = count;
//            //context.DomainContext.DataBag.TotalPage = (int)Math.Ceiling((count / (double)size));
//        }
//    }
//}
