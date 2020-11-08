import React from 'react';
import ReactDOM from 'react-dom';
import { Provider } from 'react-redux';
import store from './store';
import Routes from './routes.jsx';
import './index.css';
import 'react-datepicker/dist/react-datepicker.css';
import registerServiceWorker from './registerServiceWorker';
import * as ErrorActions from './actions/errorActions';
import Info from './components/Info/Info';

window.onerror = (errorMessage, url, lineNumber, columnNumber, error) => 
{
  ErrorActions.logError(errorMessage, error.stack, url, columnNumber, lineNumber);
}

const app = (
  <Provider store={store}>
    <Routes />
    <Info/>
  </Provider>
);

ReactDOM.render(app, document.getElementById('root'));
registerServiceWorker();
