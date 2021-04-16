import React, { Component } from 'react';
import _ from 'lodash';
import PropTypes from 'prop-types';
import RBModal from 'react-bootstrap/Modal';
import Button from 'react-bootstrap/Button';
import * as PageActions from '../../actions/pageActions';
import * as PageUtils from '../../js/pageUtils';
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
      this.callAction = this.callAction.bind(this);
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

    /**
     * Page action call
     */
    callAction()
    {
      const { componentsToInclude, pageComponents, page, action, onClose } = this.props;
      const parameters = {...this.state.actionResponseValues};

      if (!_.isNil(componentsToInclude) && componentsToInclude.length > 0)
      {
        PageUtils.setIncludedComponents(pageComponents, parameters, componentsToInclude);
      }

      PageActions.callAction(page, action, parameters, onClose);
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
              this.callAction();
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
                  this.setState({
                      confirm: {
                        show: false,
                        action: '',
                        actionDataKeys: []
                      }
                    })
                  this.callAction();
                }}
                variant={props.confirmVariant}/>
              <PageContent 
                onFieldValueChange={this.onChangeInputValue}
                components={props.components}
                getButtonClick={this.props.getPageButtonClick}
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
  data: PropTypes.object,
  /** Components from page to be included in action */
  componentsToInclude: PropTypes.arrayOf(PropTypes.string),
  /** Components of page. Needed if components are to be included in page action call */
  pageComponents: PropTypes.array,
  getPageButtonClick: PropTypes.func
};

export default Modal;