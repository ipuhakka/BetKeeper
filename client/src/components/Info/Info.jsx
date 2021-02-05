import React, { Component } from 'react';
import { connect } from 'react-redux';
import {clearAlert} from '../../actions/alertActions';
import Toast from 'react-bootstrap/Toast';
import './Info.css';
import _ from 'lodash';

class Info extends Component
{
	render()
	{
		let style = '';

		if (this.props.status >= 200 && this.props.status < 300)
		{
			style = 'success';
		}

		if (this.props.status >= 400 || this.props.status === -1)
		{
			style = "warning";
		}
		if (this.props.status >= 500 || this.props.status === 0)
		{
			style = "danger";
		}

		const { props } = this;
		return <Toast 
			onClose={this.dismiss} 
			show={!_.isNil(props.status)} 
			autohide 
			delay={2500}
			className={`info ${style}${_.isNil(props.status) ? ' hidden' : ''}`}>
		<Toast.Header>
		  <strong className="mr-auto">Betkeeper</strong>
		  <small>{new Date().toLocaleDateString()}</small>
		</Toast.Header>
		<Toast.Body>{props.statusMessage}</Toast.Body>
	  </Toast>;
	}

	dismiss = () => 
	{
		this.props.clearAlert();
	}
}

const mapStateToProps = (state) => {
  return { ...state.alert}
};

const mapDispatchToProps = (dispatch) => ({
	clearAlert: () => dispatch(clearAlert())
});

export default connect(mapStateToProps, mapDispatchToProps)(Info);
