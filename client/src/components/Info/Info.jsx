import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import {clearAlert} from '../../actions/alertActions';
import Alert from 'react-bootstrap/lib/Alert';
import Button from 'react-bootstrap/lib/Button';

class Info extends Component{

	render(){
		return(<div>{this.renderAlert()}</div>);
	}

	//This function uses a switch to return a type of alert or null. Switch is prop 'alertState'. Its values are statuscodes of
	//http-requests. Other alert types use code -1. Any other value returns no alert element.
	renderAlert = () => {
		let style = "success";
		if (this.props.status >= 400 || this.props.status === -1){
			style = "warning";
		}
		if (this.props.status >= 500 || this.props.status === 0){
			style = "danger";
		}

		if (this.props.status !== null){
			if (this.props.timeout > 0){
				setTimeout(() => {
					this.dismiss();
				}, this.props.timeout);
			}

			return(<Alert bsStyle={style} onDismiss={this.dismiss}>
					<p>{this.props.statusMessage}</p>
					<Button onClick={this.dismiss}>{"Hide"}</Button>
					</Alert>);
		} else {
			return null;
		}
	}

	dismiss = () => {
		this.props.clearAlert();
	}

}

Info.defaultProps = {
	timeout: 2000 //number of milliseconds alert is visible. If set to 0 or less, alert is closed only manually
};

Info.propTypes = {
	timeout: PropTypes.number
};

const mapStateToProps = (state, ownProps) => {
  return { ...state.alert}
};

const mapDispatchToProps = (dispatch) => ({
	clearAlert: () => dispatch(clearAlert())
});

export default connect(mapStateToProps, mapDispatchToProps)(Info);
