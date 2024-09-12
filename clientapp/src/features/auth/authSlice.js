import { createSlice } from '@reduxjs/toolkit';

// Define the initial state of the auth
const initialState = {
  user: null,
  token: localStorage.getItem("token") || null,
  error: null,
  loading: false,
};

// Create a slice for authentication
const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    loginStart: (state) => {
      state.loading = true;
      state.error = null;
    },
    loginSuccess: (state, action) => {
      state.loading = false;
      state.error = null;
      state.user = action.payload.user;
      state.token = action.payload.token;
      localStorage.setItem('token', action.payload.token);
    },
    loginFailure: (state, action) => {
      state.loading = false;
      state.error = action.payload;
    },
    logout: (state) => {
      state.user = null;
      state.error = null;
      state.token = null;
      localStorage.removeItem('token');
    },
  },
});

// Export actions to be used in the components
export const { loginStart, loginSuccess, loginFailure, logout } = authSlice.actions;

// Export the reducer to configure the store
export default authSlice.reducer;
