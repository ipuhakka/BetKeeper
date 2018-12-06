import React, { Component } from 'react';
import { connect } from 'react-redux';
import {clearAlert} from '../../actions/alertActions';
import Alert from 'react-bootstrap/lib/Alert';
import Button from 'react-bootstrap/lib/Button';

class Info extends Component{

	render(){
		var alert = this.renderAlert();
		return(<div>{alert}</div>);
	}

	//This function uses a switch to return a type of alert or null. Switch is prop 'alertState'. Its values are statuscodes of
	//http-requests. Other alert types use code -1. Any other value returns no alert element.
	renderAlert = () => {
		let style = "success";
		if (this.props.status >= 400){
			style = "warning";
		}
		if (this.props.status >= 500 || this.props.status === 0){
			style = "danger";
		}

		if (this.props.status !== null){
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

const mapStateToProps = (state, ownProps) => {
  return { ...state.alert}
};

const mapDispatchToProps = (dispatch) => ({
	clearAlert: () => dispatch(clearAlert())
});

export default connect(mapStateToProps, mapDispatchToProps)(Info);
