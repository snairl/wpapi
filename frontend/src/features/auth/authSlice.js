import { createSlice } from "@reduxjs/toolkit";

const authSlice = createSlice({
  name: "auth",
  initialState: {
    isAuthenticated: false,
    user: null,
    token: null
  },
  reducers: {
    setCredentials: (state, action) => {
      const { user, token } = action.payload;
      state.user = user;
      state.token = token;
      state.isAuthenticated = true;
    },
    logout: (state, action) => {
      state.user = null;
      state.token = null;
      state.isAuthenticated = false; 
    },
  },
});


export const { setCredentials, logout } = authSlice.actions;


export default authSlice.reducer;

export const selectCurrentUser = (state) => state.auth.user; 
export const selectCurrentToken = (state) => state.auth.token;