import 'dart:async';

import 'package:easy_localization/easy_localization.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/router/app_router.dart';
import '../bloc/auth_bloc.dart';
import '../bloc/auth_event.dart';
import '../bloc/auth_state.dart';

class MfaScreen extends StatefulWidget {
  final String sessionToken;

  const MfaScreen({super.key, required this.sessionToken});

  @override
  State<MfaScreen> createState() => _MfaScreenState();
}

class _MfaScreenState extends State<MfaScreen> {
  final _otpCtrl = TextEditingController();
  int _resendCountdown = 60;
  Timer? _timer;

  @override
  void initState() {
    super.initState();
    _startTimer();
  }

  void _startTimer() {
    _resendCountdown = 60;
    _timer?.cancel();
    _timer = Timer.periodic(const Duration(seconds: 1), (t) {
      if (_resendCountdown == 0) {
        t.cancel();
      } else {
        setState(() => _resendCountdown--);
      }
    });
  }

  @override
  void dispose() {
    _timer?.cancel();
    _otpCtrl.dispose();
    super.dispose();
  }

  void _verify() {
    final otp = _otpCtrl.text.trim();
    if (otp.length != 6) return;
    context.read<AuthBloc>().add(
          MfaVerified(sessionToken: widget.sessionToken, otp: otp),
        );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('auth.mfa.enter_otp'.tr()),
        leading: BackButton(onPressed: () => context.go(AppRoutes.login)),
      ),
      body: BlocConsumer<AuthBloc, AuthState>(
        listener: (context, state) {
          if (state is AuthAuthenticated) {
            context.go(AppRoutes.home);
          } else if (state is AuthFailure) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(content: Text(state.failure.message)),
            );
          }
        },
        builder: (context, state) {
          return SafeArea(
            child: Padding(
              padding: const EdgeInsets.symmetric(horizontal: 24),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  const SizedBox(height: 40),
                  const Icon(Icons.sms, size: 64, color: Colors.blue),
                  const SizedBox(height: 16),
                  Text(
                    'auth.mfa.enter_otp'.tr(),
                    style: Theme.of(context).textTheme.titleLarge,
                    textAlign: TextAlign.center,
                  ),
                  const SizedBox(height: 32),

                  // 6-digit OTP field
                  TextField(
                    controller: _otpCtrl,
                    keyboardType: TextInputType.number,
                    textAlign: TextAlign.center,
                    maxLength: 6,
                    inputFormatters: [FilteringTextInputFormatter.digitsOnly],
                    style: const TextStyle(
                      fontSize: 28,
                      letterSpacing: 12,
                      fontWeight: FontWeight.bold,
                    ),
                    decoration: const InputDecoration(
                      counterText: '',
                      border: OutlineInputBorder(),
                    ),
                    onChanged: (v) {
                      if (v.length == 6) _verify();
                    },
                  ),
                  const SizedBox(height: 24),

                  FilledButton(
                    onPressed: state is AuthLoading ? null : _verify,
                    child: state is AuthLoading
                        ? const SizedBox(
                            height: 20,
                            width: 20,
                            child: CircularProgressIndicator(
                              strokeWidth: 2,
                              color: Colors.white,
                            ),
                          )
                        : Text('auth.mfa.enter_otp'.tr()),
                  ),
                  const SizedBox(height: 16),

                  // Resend
                  TextButton(
                    onPressed: _resendCountdown == 0
                        ? () {
                            _otpCtrl.clear();
                            _startTimer();
                            // Re-trigger login to resend OTP would require going back
                          }
                        : null,
                    child: Text(
                      _resendCountdown > 0
                          ? '${'auth.mfa.resend'.tr()} ($_resendCountdown s)'
                          : 'auth.mfa.resend'.tr(),
                    ),
                  ),
                ],
              ),
            ),
          );
        },
      ),
    );
  }
}
