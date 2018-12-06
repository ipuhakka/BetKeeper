import React from 'react';
import ReactDOM from 'react-dom';
import { Provider } from 'react-redux';
import store from './store';

export function changeToComponent(component){
  ReactDOM.render(<Provider store={store}>
                    {component}
                  </Provider>, document.getElementById('root'));
}
