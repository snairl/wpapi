import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";

import { setCredentials, logout } from "../../features/auth/authSlice";

const baseQuery = fetchBaseQuery({ 
    baseUrl: "http://localhost:3001",
    credentials: 'include',
    prepareHeaders: (headers, { getState }) => {
        const token = getState().auth.token;
        if (token) {
            headers.set('authorization', `Bearer ${token}`);
        }
        return headers;
    }
});

const baseQueryWithReauth = async (args, api, extraOptions) => {
    let result = await baseQuery(args, api, extraOptions);
    if (result?.error?.originalStatus === 403) {
        console.log('Reauthenticating');
        //send the refresh token to the server
        const refreshResult = await baseQuery('/auth/refresh', api, extraOptions);
        if(refreshResult?.data){
            console.log('Reauthenticated');
            const { user, token } = refreshResult.data;
            api.dispatch(setCredentials({ user, token }));
            result = await baseQuery(args, api, extraOptions);
        }else{
            api.dispatch(logout());
        }
    }
    return result;
}

export const apiSlice = createApi({
    baseQuery: baseQueryWithReauth,
    endpoints: (builder) => ({}),
});