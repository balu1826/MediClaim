const hooks = require('hooks');

hooks.beforeEach((transaction) => {
    transaction.fullPath =
        '/v1/claims/3a2bf1b5-c727-4b5e-8148-a9ea35a5664a/submit';
  transaction.request.headers['Authorization']
      = 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJkMmIxNzg2Yy05MjhkLTQ4MDktYjEyYi04MDBjOTNhNWJlZGYiLCJlbWFpbCI6InBhdGllbnRAZXhhbXBsZS5jb20iLCJ0ZW5hbnRfaWQiOiIzMTMxMDMxZS1lNWUyLTRiYmQtYjcxNy1mMzc0YzgzODcxNmYiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJQYXRpZW50IiwiZXhwIjoxNzc5NDU0OTc1LCJpc3MiOiJNZWRpQ2xhaW1BUEkiLCJhdWQiOiJNZWRpQ2xhaW1Vc2VycyJ9.cpe-GHtmdxP3s0QJxnKjl0Ul5XixGw8rX5QN_P5w024';

});// JavaScript source code
