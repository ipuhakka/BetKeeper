import React, { Component } from 'react';
import LoginView from './views/LoginView/LoginView.jsx';
import store from './store';
import { Provider } from 'react-redux';
import './App.css';

class App extends Component {
	render() {
		return (
		<div className="App">
			<Provider store={store}>
				<LoginView></LoginView>
			</Provider>
		</div>
		);
	}
}

export default App;
