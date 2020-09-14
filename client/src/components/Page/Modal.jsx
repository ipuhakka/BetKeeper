import React, { Component } from 'react';
import _ from 'lodash';
import PropTypes from 'prop-types';
import RBModal from 'react-bootstrap/Modal';
import Button from 'react-bootstrap/Button';
import * as PageActions from '../../actions/pageActions';
import Confirm from '../Confirm/Confirm';
import PageContent from '../Page/PageContent';

/** Action modal. */
class Modal extends Component 
{
    constructor(props)
    {
      super(props);

      this.state = {
        actionResponseValues: {},
        hasInvalidInputs: false,
        confirm: {
          show: false,
          action: '',
          actionDataKeys: []
        }
      }

      this.onChangeInputValue = this.onChangeInputValue.bind(this);
    }

    onChangeInputValue(key, newValue)
    {
      // TODO: Eroon erillisestä datasäiliöstä actionille?
      const actionResponse = {...this.state.actionResponseValues};

      actionResponse[key] = newValue;

      this.setState({
        actionResponseValues: actionResponse,
        hasInvalidInputs: false
      });

      PageActions.onDataChange(this.props.page, key, newValue);
    }

    render()
    {
        const { props, state } = this;

        const acceptButton = _.isNil(props.action) 
          || props.action.length === 0
          ? null
          : <Button 
          variant="outline-primary"
          disabled={state.hasInvalidInputs}
          onClick={() => 
          {
            if (props.requireConfirm)
            {
              this.setState({
                confirm: {
                  ...this.state.confirm,
                  show: true
                }
              })
            }
            else 
            {
              PageActions.callAction(props.page, props.action, state.actionResponseValues, props.onClose);
            }
          }}>Ok</Button>;
        
        return <RBModal show={props.show} onHide={props.onClose}>
          <RBModal.Header closeButton>
            <RBModal.Title>{props.title}</RBModal.Title>
          </RBModal.Header>
        
          <RBModal.Body>
            <div>
            <Confirm 
                visible={state.confirm.show}
                headerText={props.title}
                cancelAction={() => 
                {
                    this.setState({
                        confirm: {
                            show: false,
                            action: '',
                            actionDataKeys: []
                        }
                    });
                }}
                confirmAction={() => 
                {
                  PageActions.callAction(props.page, props.action, state.actionResponseValues, props.onClose)
                }}
                variant={props.confirmVariant}/>
              <PageContent 
                onFieldValueChange={this.onChangeInputValue}
                components={props.components}
                getButtonClick={() => { throw new Error('No button click')}}
                className='modal-content'
                absoluteDataPath={props.absoluteDataPath}
                data={props.data}
                />
            </div>
          </RBModal.Body>
        
          <RBModal.Footer>
            <Button 
              variant="outline-secondary" 
              onClick={props.onClose}>Close</Button>
            {acceptButton}
          </RBModal.Footer>
        </RBModal>;
    }
};

Modal.defaultProps = {
  action: '',
  components: [],
  title: '',
  requireConfirm: false
};

Modal.propTypes = {
  onClose: PropTypes.func.isRequired,
  show: PropTypes.bool.isRequired,
  page: PropTypes.string.isRequired,
  action: PropTypes.string.isRequired,
  title: PropTypes.string.isRequired,
  requireConfirm: PropTypes.bool.isRequired,
  confirmVariant: PropTypes.string,
  components: PropTypes.array.isRequired,
  absoluteDataPath: PropTypes.string,
  data: PropTypes.object
};

export default Modal;