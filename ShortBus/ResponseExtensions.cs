namespace ShortBus
{
    using System;
    using System.Threading.Tasks;

    public static class ResponseExtensions
    {
        public static Response NotifyWithResponse<TNotification>(this IMediator mediator,
            TNotification notification)
        {
            var response = new Response();

            try {
                mediator.Notify(notification);
            } catch (Exception e) {
                response.Exception = e;
            }

            return response;
        }

        public static async Task<Response> NotifyWithResponseAsync<TNotification>(this IMediator mediator,
            TNotification notification)
        {
            var response = new Response();

            try {
                await mediator.NotifyAsync(notification).ConfigureAwait(false);
            } catch (Exception e) {
                response.Exception = e;
            }

            return response;
        }

        public static Response<TResponseData> RequestWithResponse<TResponseData>(this IMediator mediator,
            IRequest<TResponseData> request)
        {
            var response = new Response<TResponseData>();

            try {
                response.Data = mediator.Request(request);
            } catch (Exception e) {
                response.Exception = e;
            }

            return response;
        }

        public static async Task<Response<TResponseData>> RequestWithResponseAsync<TResponseData>(
            this IMediator mediator, IAsyncRequest<TResponseData> request)
        {
            var response = new Response<TResponseData>();

            try {
                response.Data = await mediator.RequestAsync(request).ConfigureAwait(false);
            } catch (Exception e) {
                response.Exception = e;
            }

            return response;
        }
    }
}