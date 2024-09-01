import { apiSlice } from "../../app/api/apiSlice";

export const authApiSlice = apiSlice.injectEndpoints({
    endpoints: Builder => ({
        login: Builder.mutation({
            query: credentials => ({
                url: 'http://localhost:5171/api/login',
                method: 'POST',
                body: {...credentials}
            })
        })
    })
});

export const { useLoginMutation } = authApiSlice;
