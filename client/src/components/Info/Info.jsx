import React, { Component } from 'react';
import { connect } from 'react-redux';
import {clearAlert} from '../../actions/alertActions';
import Alert from 'react-bootstrap/lib/Alert';
import {Transition} from 'react-transition-group';
import './Info.css';

const TIME_VISIBLE = 1600;

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

		const transitionStyles = {
			entering: {opacity: 1, display: 'inherit'},
			exited: {opacity: 0, display: 'none'}
		}

		return(
				<Transition in={this.props.status !== null}
					timeout={700}>
					{status => (
						<Alert className="info" style={{...transitionStyles[status]}} bsStyle={style} onDismiss={this.dismiss}>
							<p>{this.props.statusMessage}</p>
						</Alert>
					)}
			</Transition>);
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
