import React, { Component } from 'react';
import { connect } from 'react-redux';
import {clearAlert} from '../../actions/alertActions';
import Alert from 'react-bootstrap/lib/Alert';
import Button from 'react-bootstrap/lib/Button';
import './Info.css';

const TIME_VISIBLE = 4000;

class Info extends Component{

	render(){

		let style = "success";
		if (this.props.status >= 400 || this.props.status === -1){
			style = "warning";
		}
		if (this.props.status >= 500 || this.props.status === 0){
			style = "danger";
		}

		if (this.props.status !== null){
			setTimeout(() => {
				this.dismiss();
			}, TIME_VISIBLE);
		}

		return(<Alert className={this.props.status !== null ? 'visible' : 'hidden'} bsStyle={style} onDismiss={this.dismiss}>
					<p>{this.props.statusMessage}</p>
					<Button onClick={this.dismiss}>{"Hide"}</Button>
				</Alert>);
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
